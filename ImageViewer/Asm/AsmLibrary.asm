.data
alphaMask byte 4 dup(0FFh, 0FFh, 0FFh, 000h)
negAlphaMask byte 4 dup(000h, 000h, 000h, 0FFh)
array255 byte 16 dup(0FFh)
treshold qword 010h
middle DWORD 8 dup(128.0)
masktable DWORD 2 dup(-0.0, -0.0, -0.0, 0.0)
highval DWORD 8 dup(255.0)
lowval DWORD 8 dup(0.0)
perms BYTE 02h, 06h, 0ah, 0eh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh
cperms BYTE 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 0fh, 02h, 06h, 0ah, 0eh
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

asmContrastFilterPro proc
	mov r9, rcx
	mov r11, rcx
	add r11, rdi
	xor r12, r12
	VPSHUFD xmm4, xmm1, 00h
	VDIVPS xmm4, xmm4, xmm4
	VPSLLDQ xmm4, xmm4, 0ch
	vmovupd xmm0, [r9]												; load 16 bytes
	VPSHUFD xmm1, xmm1, 0c0h										; load coefficient and vectorize
	VPADDB xmm1, xmm1, xmm4
	VXORPS xmm4, xmm4, xmm4											; clean temp array
firstloop :
	VPSRLDQ xmm4, xmm4, 04h											; shift right to make place for next 4 bytes
	VPMOVZXBD xmm2, xmm0											; move with zero extend low 4 bytes
	VPSRLDQ xmm0, xmm0, 04h											; shift data chunk left by 4 bytes
	VCVTDQ2PS xmm2, xmm2											; convert to float
	VSUBPS xmm2, xmm2, middle
	VMULPS xmm2, xmm2, xmm1
	VADDPS xmm2, xmm2, middle
	VROUNDPS xmm2, xmm2, 1
	VCMPPS xmm3, xmm2, highval, 5									; generate mask if overflow
	VBLENDVPS xmm2, xmm2, highval, xmm3								; substitute values higher than 255
	VCMPPS xmm3, xmm2, lowval, 1									; generate mask if overflow
	VBLENDVPS xmm2, xmm2, lowval, xmm3								; substitute values lower than 0
	VCVTPS2DQ xmm2, xmm2 ;convert to dword integer
	VPSLLDQ xmm2, xmm2, 02h ;shift 2 byte left
	VPSHUFB xmm2, xmm2, xmmword ptr[cperms]							; permute
	VPADDB xmm4, xmm4, xmm2											; store in temp register
	inc r12
	cmp r12, 04h
	jl firstloop
	vmovupd [r9], xmm4
	add r9, 010h
	xor r12, r12		
	mov edx, 0														; clear dividend, high
	mov rax, rdi ; dividend, low
	mov ecx, 010h ; divisor
	div ecx ;
	cmp rdx, 0
	je mainloop
	sub r9, 010h
	add r9, rdx
mainloop :
	vmovupd xmm0, [r9]
	VXORPS xmm4, xmm4, xmm4
	xor r12, r12
helploop :
	VPSRLDQ xmm4, xmm4, 04h											; shift right to make place for next 4 bytes
	VPMOVZXBD xmm2, xmm0											; move with zero extend low 4 bytes
	VPSRLDQ xmm0, xmm0, 04h											; shift data chunk left by 4 bytes
	VCVTDQ2PS xmm2, xmm2											; convert to float
	VSUBPS xmm2, xmm2, middle
	VMULPS xmm2, xmm2, xmm1
	VADDPS xmm2, xmm2, middle
	VROUNDPS xmm2, xmm2, 1
	VCMPPS xmm3, xmm2, highval, 5									; generate mask if overflow
	VBLENDVPS xmm2, xmm2, highval, xmm3								; substitute values higher than 255
	VCMPPS xmm3, xmm2, lowval, 1									; generate mask if overflow
	VBLENDVPS xmm2, xmm2, lowval, xmm3								; substitute values lower than 0
	VCVTPS2DQ xmm2, xmm2 ;convert to dword integer
	VPSLLDQ xmm2, xmm2, 02h ;shift 2 byte left
	VPSHUFB xmm2, xmm2, xmmword ptr[cperms]							; permute
	VPADDB xmm4, xmm4, xmm2											; store in temp register
	inc r12
	cmp r12, 04h
	jl helploop
	vmovupd [r9], xmm4	
	add r9, 010h
	cmp r9, r11
	jl mainloop
	ret
asmContrastFilterPro endp

asmByteToFloat proc
	mov r9, rcx
	mov r11, rcx
	add r11, rbx
	mov r10, rdx
	xor r12, r12
	vmovupd xmm0, [r9]
firstloop :
	VPMOVZXBD xmm1, xmm0
	VCVTDQ2PS xmm1, xmm1
	vmovupd [r10], xmm1
	VPSRLDQ xmm0, xmm0, 04h
	add r10, 010h
	inc r12
	cmp r12, 04h
	jl firstloop
	add r9, 010h
	xor r12, r12		
	mov edx, 0 ; clear dividend, high
	mov eax, ebx ; dividend, low
	mov ecx, 010h ; divisor
	div ecx ;
	cmp rdx, 0
	je mainloop
	sub r9, 010h
	add r9, rdx
	sub r10, 040h
	mov eax, 04h
	mul rdx
	add r10, rax
mainloop :
	vmovupd xmm0, [r9]
helploop :
	VPMOVZXBD xmm1, xmm0
	VCVTDQ2PS xmm1, xmm1
	vmovupd [r10], xmm1
	VPSRLDQ xmm0, xmm0, 04h
	add r10, 010h
	inc r12
	cmp r12, 04h
	jl helploop
	xor r12, r12
	add r9, 010h
	cmp r9, r11
	jl mainloop
	ret
asmByteToFloat endp

asmFloatToByte proc
	mov r9, rcx
	mov r8, rcx
	mov r11, rcx
	mov r10, rdx
	xor r12, r12
	mov eax, 04h
	mul rbx
	add r11, rax
	vmovupd xmm2, xmmword ptr[perms]
firstloop :
	vmovupd xmm0, [r9]
	VCVTPS2DQ xmm0, xmm0
	VPSLLDQ xmm0, xmm0, 02h
	VPSHUFB xmm1, xmm0, xmm2
	VPADDB xmm3, xmm3, xmm1
	VPSLLDQ xmm2, xmm2, 04h
	add r9, 010h
	inc r12
	cmp r12, 04h
	jl firstloop
	vmovupd [r10], xmm3
	xor r12, r12
	mov edx, 0 ; clear dividend, high
	mov eax, ebx ; dividend, low
	mov ecx, 010h ; divisor
	div ecx ;
	cmp rdx, 0
	je mainloop
	mov eax, 04h
	add r10, rdx
	mul rdx
	add r8, rax
	VXORPS xmm3, xmm3, xmm3
mainloop :
	vmovupd xmm2, xmmword ptr[perms]
workloop :
	vmovupd xmm0, [r8]
	VCVTPS2DQ xmm0, xmm0
	VPSLLDQ xmm0, xmm0, 02h
	VPSHUFB xmm1, xmm0, xmm2
	VPADDB xmm3, xmm3, xmm1
	VPSLLDQ xmm2, xmm2, 04h
	add r8, 010h
	inc r12
	cmp r12, 04h
	jl workloop
	vmovupd [r10], xmm3
	xor r12, r12
	VXORPS xmm3, xmm3,xmm3
	add r10, 010h
	cmp r8, r11
	jl mainloop
	ret
asmFloatToByte endp


end