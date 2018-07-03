.data
alphaMask byte 4 dup(0FFh, 0FFh, 0FFh, 000h) ; 255, 255, 255, 0
negAlphaMask byte 4 dup(000h, 000h, 000h, 0FFh) ; 0, 0, 0, 255
array255 byte 16 dup(0FFh) ; 255
treshold qword 010h ; 16
middle DWORD 8 dup(128.0)
masktable DWORD 2 dup(-0.0, -0.0, -0.0, 0.0)
highval DWORD 8 dup(255.0)
lowval DWORD 8 dup(0.0)
colour qword 004h
val3 DWORD 2 dup(3.0)
val255 DWORD 2 dup(255.0)
greenLimit DWORD 2 dup(195.0)
redLimit DWORD 2 dup(225.0)

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
	mov r10, 0 ; r10 = 0
	mov r11, rbx ; r11 = rbx
	mov r9, rdx ; r9 = rdx
	jmp loopEnd ; go to loopEnd

loopStart :
	vmovupd xmm1, xmmword ptr[rcx + r10] ; red
	add r10, colour ; red to green
	vmovupd xmm2, xmmword ptr[rcx + r10] ; green
	add r10, colour ; green to blue
	vmovupd xmm3, xmmword ptr[rcx + r10] ; blue
	vaddpd xmm0, xmm0, xmm1
	vaddpd xmm0, xmm0, xmm2
	vmovups xmm3, val3
	vdivpd xmm0, xmm0, xmm3
	vmovups xmm0, val255
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
	add r10, treshold ; r10 = r10 + 16
loopEnd :
	cmp r10, r9 ; compare r10 (0) and r9 ; how much of the image left
	jl loopStart ; if r10 (0) < r9, go to loopStart
	mov rbx, r11 ; rbx = r11
	ret

tooMuchGreen :
	vmovups xmm1, val255
	jmp greensGood

tooMuchRed :
	vmovups xmm1, val255
	jmp redsGood

asmSepiaFilter endp
end