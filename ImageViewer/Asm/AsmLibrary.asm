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
three dword 4 dup(3.0)
thirty dword 4 dup(30.0)
limit dword 1 dup(255.0, 255.0, 255.0, 255.0)
bitMask byte 4 dup(0FFh, 0FFh, 000h, 000h)


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


asmSepiaFilter proc
	; parameters processing
	mov r9, rdx		; size
	mov r11, rbx	; bitmap

	; setup
	mov r10, 0			; current position in bitmap

	jmp loopEnd

loopStart :
	sub r10, treshold ; there is enough bits left to be processed

	vmovups xmm0, xmmword ptr[rcx+r10] ; taking 16 bytes from the bitmap - that's 4 bits per pixel, values RGBA
	; actual sequence of values in register: ARGB ARGB ARGB ARGB

	vmovups xmm1, xmm0
	vmovups xmm2, xmm0
	vmovups xmm3, xmm0
	vmovups xmm4, xmm0

	vmovups xmm5, xmmword ptr[negAlphaMask] ; inconsistent naming scheme stems from different teams working on different filters, but this is the exact mask needed here so we'll take it
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
	
	; blue
	movups xmm4, xmm0
	cvtps2dq xmm4, xmm4
	
	vmovups xmm5, limit
	vmovups xmm6, thirty
	
	; green
	vaddps xmm0, xmm0, xmm6
	movups xmm3, xmm0
	minps xmm3, xmm5
	cvtps2dq xmm3, xmm3
	pslldq xmm3, 1
	
	; red
	vaddps xmm0, xmm0, xmm6
	movups xmm2, xmm0
	minps xmm2, xmm5
	cvtps2dq xmm2, xmm2
	pslldq xmm2, 2
	
	vpaddb xmm0, xmm1, xmm2 ; alpha + red
	vpaddb xmm0, xmm0, xmm3 ; + green
	vpaddb xmm0, xmm0, xmm4 ; + blue
	; xmm0 is ready to be returned to bitmap

	vmovups xmmword ptr[rcx+r10], xmm0 ; returns modified pixels back

	add r10, treshold
loopEnd :
	add r10, treshold	; test if there is 16 more bits in image (4 pixels)
	cmp r10, r9
	jle loopStart		; is yes, loop
	mov rbx, r11		; if not, end processing the image
	ret					; this will leave between 0 to 15 last bits unprocessed if number of bits in the bitmap is not a multiple of 16 (4 pixels)

asmSepiaFilter endp
end