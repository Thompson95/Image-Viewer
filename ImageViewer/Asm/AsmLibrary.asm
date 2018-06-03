.data
alphaMask byte 4 dup(0FFh, 0FFh, 0FFh, 000h)
negAlphaMask byte 4 dup(000h, 000h, 000h, 0FFh)
array255 byte 16 dup(0FFh)
treshold qword 010h
middle DWORD 8 dup(128.0)
masktable DWORD 2 dup(-0.0, -0.0, -0.0, 0.0)
highval DWORD 8 dup(255.0)
lowval DWORD 8 dup(0.0)
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

asmSepiaFilter PROC bitmap : dword, stop : dword
pushfq 

mov r10, 0
mov r11, rbx
mov r9, rdx
mov esi, 3
; Loading bitmap to edi register
mov edi, bitmap

; Loading bitmap size to ecx register
mov ecx, stop

vpshufd xmm3, xmm3, 00h
vpshufd xmm5, xmm5, 00h
vpshufd xmm6, xmm6, 00h

; Parsing into float to allow for vector division
cvtdq2ps xmm4, xmm5 

startloop:
MOVDQU xmm0, [edi]
vmovaps xmm1, xmm0
; Loading alpha of 4 next pixels into xmm7 register
vmovaps xmm7, xmm0 
pand xmm7, xmm6
psrldq xmm1, 1
vmovaps xmm2, xmm1
psrldq xmm2, 1
pand xmm0, xmm3 ; Zeroing a,g,b
pand xmm1, xmm3 ; Zeroing skladowych a,r,b
pand xmm2, xmm3 ; Zeroing a,r,g
paddd xmm0, xmm1 ; Summing r, g
paddd xmm0, xmm2 ; Summing r, g, b
; Dividing 3 next pixels' values by 3
vcvtdq2ps xmm1, xmm0 ; Parsing int into float
divps xmm1, xmm4 ; Vector division
vcvtps2dq xmm0, xmm1 ; Parsing float into int
; Filling register with 0, g, g, g gdzie g = (r + b + g) / 3
vmovaps xmm1, xmm0
pslldq xmm0, 1
por xmm1, xmm0
pslldq xmm0, 1

; Copy alpha
por xmm1, xmm0
por xmm1, xmm7

; 4 pixels sent to results
movdqu [edi], xmm1

; 16 bits processed per loop
add edi, 16
sub ecx, 15


loop startloop

mov edi, bitmap
mov ecx, stop


; Coloring for sepia
startloop2:

; Increase green
mov al,[edi+1]
cmp al, 215 ; Avoid overflowing 255
ja ifbigger1
add al, 30
jmp next1

ifbigger1:
mov al, 255 ; Colour value cannot be bigger than 255

next1:
mov [edi+1], al


; Double increase of red
mov al,[edi+2]
cmp al, 185 ; Avoid overflowing 255
ja ifbigger2
add al, 60
jmp next2

ifbigger2:
mov al, 255 ; Colour value cannot be bigger than 255

next2:
mov [edi+2], al

add edi, 4
sub ecx, 3
loop startloop2


popfq
ret
asmSepiaFilter endp

end