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