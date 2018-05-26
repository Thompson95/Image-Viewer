.data
.code
asmNegativeFilter proc
	mov r10, 0
	mov r9, rdx
	jmp loopEnd

loopStart:
	mov rax, r10
	mov rbx, 4
	div ebx
	cmp dx, 3
	jbe notAlpha
	inc r10
	jmp loopStart
	
notAlpha:
	mov rax, 255
	sub al, byte ptr[rcx + r10]
	mov byte ptr[rcx + r10], al
	inc r10
loopEnd:
	cmp r10, r9 
	jl loopStart

	ret
asmNegativeFilter endp
end