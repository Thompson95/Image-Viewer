.data
middle DWORD 8 dup(128.0)

masktable DWORD 2 dup(-0.0, -0.0, -0.0, 0.0)

.code
asmNegativeFilter proc
	mov r10, 0
	mov r11, rbx
	mov r9, rdx
	mov r8, rcx
	mov rbx, 4
	jmp loopEnd

	loopStart :
	mov rdx, 0
	mov rax, r10
	div rbx
	cmp rdx, 3
	je Alpha

	mov rax, 255
	sub al, byte ptr[r8 + r10]
	mov byte ptr[r8 + r10], al
alpha:
	inc r10
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
	mov al, byte ptr [rcx + r10]
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
end

asmContrastFilter proc
	mov r9, rcx
	mov r10, 0
	mov r11, rcx
	add r11, rbx
mainloop :
	vmovupd ymm0, [r9] ;load chunk of data
	vmovupd ymm1, [rdx] ;load coefficient
	vmovaps ymm2, middle ;load middle value for calculation
	vmovaps ymm3, masktable ;load masked table
	VSUBPS  ymm0, ymm0, ymm2
	VMULPS ymm0, ymm0, ymm1
	VADDPS ymm0, ymm0, ymm2
	VROUNDPS ymm0, ymm0, 1
	vmaskmovps [r9], ymm3, ymm0
	add r9, 32
	cmp r9, r11
	jl mainloop
	ret
asmContrastFilter endp