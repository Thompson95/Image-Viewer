.data
alphaMask byte 4 dup(0FFh, 0FFh, 0FFh, 000h) ; 255, 255, 255, 0
negAlphaMask byte 4 dup(000h, 000h, 000h, 0FFh) ; 0, 0, 0, 255
array255 byte 16 dup(0FFh) ; 255
treshold qword 010h ; 16
middle DWORD 8 dup(128.0)
masktable DWORD 2 dup(-0.0, -0.0, -0.0, 0.0)
highval DWORD 8 dup(255.0)
lowval DWORD 8 dup(0.0)

redMask byte 4 dup(000h, 000h, 0FFh, 000h)
greenMask byte 4 dup(000h, 0FFh, 000h, 000h)
blueMask byte 4 dup(0FFh, 000h, 000h, 000h)
colour qword 001h
val3 DWORD 2 dup(3.0)
val255 byte 1 dup(0FFh)
greenLimit DWORD 2 dup(195.0)
redLimit DWORD 2 dup(225.0)
three dword 1 dup(3.0, 3.0, 3.0, 3.0)
bitMask byte 1 dup(0FFh, 0FFh, 000h, 000h, 000h, 000h, 000h, 000h, 000h, 000h, 000h, 000h, 000h, 000h, 000h, 000h)


.code
asmNegativeFilter proc
	mov r10, 0
	mov r11, rbx
	mov r9, rdx
	jmp loopEnd

loopStart :
	vmovupd xmm2, xmmword ptr[rcx + r10]
	vmovaps xmm1, xmmword ptr[alphaMask]
	vmovaps xmm0, xmmword ptr[array255]
	vpsubb xmm3, xmm0, xmm2
	vpand xmm3, xmm3, xmmword ptr [alphaMask]
	vpand xmm2, xmm2, xmmword ptr [negAlphaMask]
	vpaddb xmm3, xmm3, xmm2
	vpand xmm3, xmm3, xmmword ptr [alphaMask]
	vpand xmm2, xmm2, xmmword ptr [negAlphaMask]
	vpaddb xmm3, xmm3, xmm2
	vmovupd xmmword ptr [rcx + r10], xmm3
alpha :
	add r10, treshold
loopEnd :
	cmp r10, r9
	jl loopStart
	mov rbx, r11

ret
asmNegativeFilter endp


asmBrightnessFilter proc
mov r10, 0
mov r11, rbx
mov r9, rdx
mov rbx, 4
jmp loopEnd

loopStart :
mov rdx, 0
mov rax, r10
div rbx
cmp rdx, 3
je Alpha

mov rax, 0
mov al, byte ptr[rcx + r10]
add eax, r8d
cmp rax, 255
jle noOverflow
mov rax, 255
noOverflow:
mov byte ptr[rcx + r10], al
alpha :
inc r10
loopEnd :
cmp r10, r9
jl loopStart
mov rbx, r11

ret
asmBrightnessFilter endp


asmContrastFilter proc
	mov r9, rcx
	mov r11, rcx
	add r11, rbx
mainloop :
	vmovupd ymm0, [r9] ;load chunk of data
	vmovupd ymm1, [rdx] ;load coefficient
	vmovups ymm2, middle ;load middle value for calculation
	vmovups ymm3, masktable ;load masked table
	vmovups ymm4, highval ;load upper bound
	vmovups ymm5, lowval ;load lower bound
	VSUBPS  ymm0, ymm0, ymm2
	VMULPS ymm0, ymm0, ymm1
	VADDPS ymm0, ymm0, ymm2
	VROUNDPS ymm0, ymm0, 2
	vmaskmovps [r9], ymm3, ymm0
	VCMPNLEPS ymm6, ymm0, highval
	VANDPS  ymm6, ymm6, ymm3
	vmaskmovps [r9], ymm6, ymm4
	VCMPLEPS ymm6, ymm0, lowval
	VANDPS  ymm6, ymm6, ymm3
	vmaskmovps [r9], ymm6, ymm5
	add r9, 32
	cmp r9, r11
	jl mainloop
	ret
asmContrastFilter endp


; several bits must be processed at once
; best fill xmm1 with R values, xmm2 with G values and xmm3 with B values and process them like that
asmSepiaFilter proc
	; parameters processing
	mov r9, rdx		; size
	mov r11, rbx	; bitmap

	; setup
	;mov r13, three
	;vmovupd xmm4, three
	cvtdq2pd xmm4, xmm4
	mov r10, 0			; current position in bitmap

	jmp loopEnd

loopStart :
	sub r10, treshold ; there is enough bits left to be processed

	; xmm1 ; red
	; xmm2 ; green
	; xmm3 ; blue
	vpsubb xmm1, xmm1, xmm1 ; zeroing register for red
	vpsubb xmm2, xmm2, xmm2 ; zeroing register for green
	vpsubb xmm3, xmm3, xmm3 ; zeroing register for blue
	; the sequence in which the colours are coming is actually blue, green, red, alpha
	; alpha is ignored both in extraction and rewriting of colour so it stays the same

	vmovups xmm0, xmmword ptr[rcx+r10] ; taking 16 bytes from the bitmap - that's 4 bits, each having R, G, B and alpha value

	; diagram is on the photo in the phone
	; use masks to transform xmm0 into xmm1, xmm2 and xmm3 containing R, G and B
	; shift R and G to be in the same column as B
	; add, there is now space for overflow
	; divide sums by 3 - division takes a lot of time and memory so its good to do it only once
	; copy and shift the value back into all RGB values in xmm0
	; using masks detect where R and G are greater than thresholds and set them to 255 there
	; when implemented in xmm try to implement in ymm

	vmovups xmm1, xmm0
	vmovups xmm2, xmm0
	vmovups xmm3, xmm0
	vmovups xmm4, xmm0

	vmovups xmm5, xmmword ptr[negAlphaMask] ; inconsistent naming scheme stems from different teams working on different filters
	vmovups xmm6, xmmword ptr[redMask]
	vmovups xmm7, xmmword ptr[greenMask]
	vmovups xmm8, xmmword ptr[blueMask]
	
	pminub xmm1, xmm5 ; alpha
	pminub xmm2, xmm6 ; red
	pminub xmm3, xmm7 ; green
	pminub xmm4, xmm8 ; blue

	psrldq xmm2, 2 ; shift red two bits
	psrldq xmm3, 1 ; shift green one bit

	vaddps xmm0, xmm2, xmm3
	vaddps xmm0, xmm0, xmm4

	vmovups xmm3, three ; xmm3 value needs not be kept after this point, it can be used for something else

	cvtdq2ps xmm0, xmm0
	vdivps xmm0, xmm0, xmm3
	cvtps2dq xmm0, xmm0
	
	movups xmm4, xmm0 ; blue
	pslldq xmm0, 1
	movups xmm3, xmm0 ; green
	pslldq xmm0, 1 ; red
	
	vpaddb xmm0, xmm0, xmm3
	vpaddb xmm0, xmm0, xmm4
	vpaddb xmm0, xmm0, xmm1 ; alpha



	;
	;
	;
	; this is sisd, we're doing simd

	vpminub xmm4, xmm1, xmm7
	psrldq xmm1, 4
	vpminub xmm3, xmm1, xmm7
	psrldq xmm1, 4
	vpminub xmm2, xmm1, xmm7
	psrldq xmm1, 4

	vdivps xmm1, xmm1, xmm5
	vdivps xmm2, xmm2, xmm5
	vdivps xmm3, xmm3, xmm5
	vdivps xmm4, xmm4, xmm5

	pslldq xmm2, 4
	pslldq xmm3, 8
	pslldq xmm4, 12

	addps xmm1, xmm2
	addps xmm1, xmm3
	addps xmm1, xmm4

	;
	;
	; obsolete code
	; blue
	push rax
	push rdx
	pextrb rax, xmm0, 0
	div three ; now works, but better to it as detailed above
	pinsrb xmm3, rax, 0
	pop rdx
	pop rax
	; green
	push rax
	push rdx
	pextrb rax, xmm0, 1
	div three
	pinsrb xmm2, rax, 0
	pop rdx
	pop rax
	; red
	push rax
	push rdx
	pextrb rax, xmm0, 2
	div three
	pinsrb xmm1, rax, 0
	pop rdx
	pop rax

	; blue
	push rax
	push rdx
	pextrb rax, xmm0, 4
	div three
	pinsrb xmm3, rax, 1
	pop rdx
	pop rax
	; green
	push rax
	push rdx
	pextrb rax, xmm0, 5
	div three
	pinsrb xmm2, rax, 1
	pop rdx
	pop rax
	; red
	push rax
	push rdx
	pextrb rax, xmm0, 6
	div three
	pinsrb xmm1, rax, 1
	pop rdx
	pop rax

	; blue
	push rax
	push rdx
	pextrb rax, xmm0, 8
	div three
	pinsrb xmm3, rax, 2
	pop rdx
	pop rax
	; green
	push rax
	push rdx
	pextrb rax, xmm0, 9
	div three
	pinsrb xmm2, rax, 2
	pop rdx
	pop rax
	; red
	push rax
	push rdx
	pextrb rax, xmm0, 10
	div three
	pinsrb xmm1, rax, 2
	pop rdx
	pop rax

	; blue
	push rax
	push rdx
	pextrb rax, xmm0, 12
	div three
	pinsrb xmm3, rax, 3
	pop rdx
	pop rax
	; green
	push rax
	push rdx
	pextrb rax, xmm0, 13
	div three
	pinsrb xmm2, rax, 3
	pop rdx
	pop rax
	; red
	push rax
	push rdx
	pextrb rax, xmm0, 14
	div three
	pinsrb xmm1, rax, 3
	pop rdx
	pop rax

	vpaddb xmm1, xmm1, xmm2
	vpaddb xmm1, xmm1, xmm3

	;
	;
	; old code and various tests and experiments from now on
	vmovupd xmm0, xmmword ptr[rcx+r10] ; blue
	pextrb r12, xmm0, 0
	pextrb r13, xmm0, 4
	pextrb r14, xmm0, 8
	pextrb r15, xmm0, 12
	movd xmm3, r12

	movd xmm2, r12
	vpaddb xmm1, xmm1, xmm2
	add r10, colour ; red to alpha
	vmovupd xmm0, xmmword ptr[rcx+r10] ; alpha
	pextrb r12, xmm0, 0
	pextrb r13, xmm0, 1
	pextrb r14, xmm0, 2
	pextrb r15, xmm0, 3
	movd xmm2, r12
	cvtdq2ps xmm2, xmm2		;
	cvtdq2ps xmm3, xmm3		; cvtdq2ps and cvtps2dq are used to multiply registers using mulps
	mulps xmm2, xmm3		; otherwise mulps returns 0 regardless of contents of registers
	cvtps2dq xmm2, xmm2		;
	vpaddb xmm1, xmm1, xmm2

	add r10, colour ; blue to green
	vmovupd xmm1, xmmword ptr[rcx+r10] ; green
	add r10, colour ; green to red
	vmovupd xmm3, xmmword ptr[rcx+r10] ; red
	add r10, colour ; red to alpha
	vmovupd xmm2, xmmword ptr[rcx+r10] ; alpha

	vaddpd xmm0, xmm0, xmm1
	vaddpd xmm0, xmm0, xmm2
	vmovups xmm3, val3
	vdivpd xmm0, xmm0, xmm3
	;vmovups xmm0, val255
	vmovupd xmmword ptr [rcx + r10], xmm3 ; blue
	sub r10, colour ; blue to green
	vmovupd xmmword ptr [rcx + r10], xmm2 ; green
	sub r10, colour ; green to red
	vmovupd xmmword ptr [rcx + r10], xmm1 ; red
	vmovupd xmm1, xmm0
	vmovups xmm3, greenLimit
	cmpps xmm1, xmm3, 6
	jl tooMuchGreen
greensGood : 
	vmovupd xmmword ptr[rcx + r10], xmm1 ; green
	sub r10, colour ; green to red
	vmovupd xmm1, xmm0
	vmovups xmm3, redLimit
	cmpps xmm1, xmm3, 6
	jl tooMuchRed
redsGood :
	vmovupd xmmword ptr[rcx + r10], xmm1 ; red

	add r10, treshold	; should be treshold!!
loopEnd :
	add r10, treshold	; test if there is 16 more bits in image
	cmp r10, r9
	jle loopStart		; is yes, loop
	mov rbx, r11		; if not, end processing the image
	ret					; this will leave between 0 to 15 last bits unprocessed if number of bits in the bitmap is not a multiple of 16

tooMuchGreen :
	;vmovups xmm1, val255 ; pshufd xmm3, xmm3, 00h ???
	jmp greensGood

tooMuchRed :
	;vmovups xmm1, val255
	jmp redsGood

asmSepiaFilter endp
end