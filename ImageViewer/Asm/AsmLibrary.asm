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

asmSepiaFilter PROC bitmap : dword, start : dword, stop : dword
pushfq 

mov r10, 0
mov r11, rbx
mov r9, rdx
mov esi, 3
;Zaladowanie adresu obrazka do rejestru edi
mov edi, bitmap

;dodanie offsetu zwiazanego z podzialem na watki
add edi, start 

;zaladowanie do ecx ilosci bitow do przetworzenia
mov ecx, stop
sub ecx, start

;movlps xmm3, color_and
;movlps xmm5, trzy
;movlps xmm6, alpha_and
vpshufd xmm3, xmm3, 00h
vpshufd xmm5, xmm5, 00h
vpshufd xmm6, xmm6, 00h

;zamiana wartosci na float w celu umozliwienia wykonania dzielenia wektorowego
cvtdq2ps xmm4, xmm5 

startloop:
MOVDQU xmm0, [edi]
vmovaps xmm1, xmm0
;zapamietanie skladowych alpha 4 kolejnych pikseli w rejestrze xmm7
vmovaps xmm7, xmm0 
pand xmm7, xmm6
psrldq xmm1, 1
vmovaps xmm2, xmm1
psrldq xmm2, 1
pand xmm0, xmm3 ; wyzerowanie skladowych a,g,b
pand xmm1, xmm3 ; wyzerowanie skladowych a,r,b
pand xmm2, xmm3 ; wyzerowanie skladowych a,r,g
paddd xmm0, xmm1 ; zsumowanie skladowych r, g
paddd xmm0, xmm2 ; zsumowanie skladowych r, g, b
;dzielenie wartosci 3 kolejnych pikseli przez 3
vcvtdq2ps xmm1, xmm0 ; konwersja 32-bitowych liczb ca³kowitych na zmiennoprzecinkowe
divps xmm1, xmm4 ; dzielenie wektorowe
vcvtps2dq xmm0, xmm1 ; konwersja wektora liczb zmiennoprzecinkowych podwójnej/pojedynczej precyzji na 32-bitowy wektor liczby ca³kowitych
;wypelnienie rejestru ci¹gami 0, g, g, g gdzie g = (r + b + g) / 3
vmovaps xmm1, xmm0
pslldq xmm0, 1
por xmm1, xmm0
pslldq xmm0, 1

;przepisanie wartosci kanalu alpha
por xmm1, xmm0
por xmm1, xmm7

;wyslanie 4 pikseli do tablicy wynikowej
movdqu [edi], xmm1

; w kazdym przebiegu petli przetwarzane jest 16 bitow
add edi, 16
sub ecx, 15


loop startloop

mov edi, bitmap
add edi, start 
mov ecx, stop
sub ecx, start


;koloryzacja na sepie
startloop2:

;zwiêkszenie koloru zielonego
mov al,[edi+1]
cmp al, 215 ;sprawdza czy po dodaniu nie zostanie przekroczone 255
ja ifbigger1
add al, 30
jmp next1

ifbigger1:
mov al, 255 ;ustawia max jeœli dodawanie przekroczy³oby max

next1:
mov [edi+1], al


;podwójne zwiêkszenie czerwonego
mov al,[edi+2]
cmp al, 185 ;sprawdza czy po dodaniu nie zostanie przekroczone 255
ja ifbigger2
add al, 60
jmp next2

ifbigger2:
mov al, 255 ;ustawia max jeœli dodawanie przekroczy³oby maxs

next2:
mov [edi+2], al

add edi, 4
sub ecx, 3
loop startloop2


popfq
ret
asmSepiaFilter endp

end