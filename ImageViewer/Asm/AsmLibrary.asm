.data
alphaMask byte 4 dup(0FFh, 0FFh, 0FFh, 000h)
negAlphaMask byte 4 dup(000h, 000h, 000h, 0FFh)
array255 byte 16 dup(0FFh)
treshold qword 010h
middle DWORD 8 dup(128.0)
masktable DWORD 2 dup(-0.0, -0.0, -0.0, 0.0)
highval DWORD 8 dup(255.0)
lowval DWORD 8 dup(0.0)
alphaMaskAVX byte 8 dup(000h, 000h, 000h, 0FFh)
redMask byte 8 dup(000h, 000h, 0FFh, 000h)
greenMask byte 8 dup(000h, 0FFh, 000h, 000h)
blueMask byte 8 dup(0FFh, 000h, 000h, 000h)
three dword 8 dup(3.0)
thirty byte 8 dup(01Eh, 000h, 000h, 000h)


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
	; parameters
	mov r9, rdx					; size
	mov r11, rbx				; bitmap

	; setup
	mov r10, 0					; current position in bitmap

	jmp loopEnd

loopStart :
	sub r10, treshold			; there is enough bits left to be processed
	sub r10, treshold			; treshold is 16, step for AVX is 32, but we save memory by reusing the data from another filter

	vmovdqu ymm0, ymmword ptr[rcx+r10]	; taking 32 bits from the bitmap - that's 4 bytes per pixel, values RGBA
	; actual sequence of values in register: ARGB ARGB ARGB ARGB ARGB ARGB ARGB ARGB for image bit #8 #7 #6 #5 #4 #3 #2 #1

	vmovdqu ymm1, ymm0
	vmovdqu ymm2, ymm0
	vmovdqu ymm3, ymm0
	vmovdqu ymm4, ymm0

	; extraction masks
	vmovdqu ymm5, ymmword ptr[alphaMaskAVX]
	vmovdqu ymm6, ymmword ptr[redMask]
	vmovdqu ymm7, ymmword ptr[greenMask]
	vmovdqu ymm8, ymmword ptr[blueMask]
	
	; extract
	vpminub ymm1, ymm1, ymm5	; alpha
	vpminub ymm2, ymm2, ymm6	; red
	vpminub ymm3, ymm3, ymm7	; green
	vpminub ymm4, ymm4, ymm8	; blue

	; align R and G values with B for addition
	vpsrldq ymm2, ymm2, 2		; shift red two positions to right
	vpsrldq ymm3, ymm3, 1		; shift green one position to right

	; R + B + G
	vaddps ymm0, ymm2, ymm3
	vaddps ymm0, ymm0, ymm4

	vmovups ymm3, three			; ymm3 value needs not be kept after this point, it can be used for something else

	vcvtdq2ps ymm0, ymm0		; convert sum of RGB to single-precision float to perform division (averaging RGB values, greyscale), no SIMD division for unsigned integers
	vdivps ymm0, ymm0, ymm3		; divide by 3
	vcvtps2dq ymm0, ymm0		; conversion from float back to unsigned integer

	
	; set new RGB values to average of old RGB values, colorizing greyscale into sepia by increasing green by 30 and red by 60 (limited by 255 of course)
	; blue
	vmovdqu ymm4, ymm0
	
	vmovdqu ymm5, ymmword ptr[blueMask]	; limit by 255, the same data as blueMask
	vmovdqu ymm6, ymmword ptr[thirty]	; +30
	
	; green
	vaddps ymm0, ymm0, ymm6
	vmovdqu ymm3, ymm0
	vpminsd ymm3, ymm3, ymm5
	vpslldq ymm3, ymm3, 1		; shift green back by 1 position
	
	; red
	vaddps ymm0, ymm0, ymm6
	vmovdqu ymm2, ymm0
	vpminsd ymm2, ymm2, ymm5
	vpslldq ymm2, ymm2, 2		; shift red back by 2 positions
	
	; putting colours into correct positions of new pixels
	vpaddb ymm0, ymm1, ymm2
	vpaddb ymm0, ymm0, ymm3
	vpaddb ymm0, ymm0, ymm4

	vmovdqu ymmword ptr[rcx+r10], ymm0 ; returns modified pixels back to bitmap

	add r10, treshold			; next loop
	add r10, treshold			; treshold is 16, we move 32 on AVX

loopEnd :
	add r10, treshold
	add r10, treshold
	cmp r10, r9					; test if there is 32 more bits in image (8 pixels)
	jle loopStart				; is yes, loop
	
	mov rbx, r11				; if not, end processing the image
	ret							; this will leave up to 7 unprocessed pixels in images with number of pixels indivisible by 8, but will guarantee bitmap of any size can be processed

asmSepiaFilter endp
end