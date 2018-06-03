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

asmSepiaFilter32 PROC bitmap : dword, start : dword, stop : dword
	pushad					; PUSHAD odk³ada wszystkie rejestry na stos. Umieszcza na stosie rejestry 32-bitowe w tej kolejnoœci: EAX, ECX, EDX, EBX, ESP, EBP, ESI, EDI.Te polecenia nie przyjmuj¹ ¿adnych operandów.
	mov esi, bitmap			; ESI <- adres bitmapy
	mov eax, stop			; adres konca przetwarzanego kawa³ka obrazka
	mov ecx, eax
	add ecx, esi
	mov ebx, start			; adres adres poczatku przetwarzanego kawa³ka obrazka
	add esi, ebx			
	

	assume esi:ptr byte		; ESI wskazuje na jeden bajt pamieci
	_loop:
			xor eax, eax	; Zerowanie rejestru
			xor ebx, ebx	; Zerowanie rejestru
			xor edx, edx	; Zerowanie rejestru
			mov al, [esi]	; AL <- skladowa R
			mov bl, [esi+1] ; BL <- skladowa G
			add eax, ebx	; EAX <- R+B
			mov bl, [esi+2]	; BL <- skladowa B
			add eax, ebx	; EAX <- R+B+G
			mov ebx, 3		; EBX <- 3 (dzielnik)
			div ebx			; AL <- R+B+G/3
			mov [esi], al	; NowaR <- AL
			mov [esi+1], al ; NowaG <- AL
			mov [esi+2], al ; NowaB <- AL
			xor eax, eax	; Zerowanie rejestru
			xor ebx, ebx	; Zerowanie rejestru
			xor edx, edx	; Zerowanie rejestru
			mov al, [esi+1] ; zielony
			mov bl, 30
			cmp al, 225   
			ja bigger
			add eax, ebx
			mov [esi+1], al
		next:	
			xor eax, eax	; Zerowanie rejestru
			xor ebx, ebx	; Zerowanie rejestru
			xor edx, edx	; Zerowanie rejestru
			mov al, [esi+1] 
			mov bl, 60
			cmp al, 195
			ja bigger2
			add eax, ebx
			mov [esi+2], al ; czerwony
		next2:
			add esi, 3
			cmp ecx, esi
			ja _loop

	endloop:
	popad					; POPAD zdejmuje ze szczytu stosu wszystkie wartoœci i kopiuje do 32-bitowych rejestrów w kolejnoœci: EDI, ESI, EBP, ESP, EBX, EDX, ECX, EAX. Ta instrukcja nie przyjmuje ¿adnych operandów.
	ret

	bigger:
	mov al, 255
	mov [esi+1], al
	jmp next

	bigger2:
	mov al, 255
	mov [esi+2], al
	jmp next2

asmSepiaFilter32 endp

asmSepiaFilter PROC bitmap : dword, start : dword, stop : dword
pushad


mov esi, 3
;Zaladowanie adresu obrazka do rejestru edi
mov edi, bitmap
;dodanie offsetu zwiazanego z podzialem na watki
add edi, start 

;zaladowanie do ecx ilosci bitow do przetworzenia
mov ecx, stop
sub ecx, start

vmovlps ymm3, color_and
vmovlps ymm5, trzy
vmovlps ymm6, alpha_and
vpshufd ymm3, ymm3, 00h
vpshufd ymm5, ymm5, 00h
vpshufd ymm6, ymm6, 00h

;zamiana wartosci na float w celu umozliwienia wykonania dzielenia wektorowego
vcvtdq2ps ymm4, ymm5 

startloop:
vmovdqu ymm0, [edi]
vmovaps ymm1, ymm0

;zapamietanie skladowych alpha 4 kolejnych pikseli w rejestrze xmm7
vmovaps ymm7, ymm0 
vpand ymm7, ymm6

vpsrldq ymm1, 1
vmovaps ymm2, ymm1
vpsrldq ymm2, 1

vpand ymm0, ymm3 ; wyzerowanie skladowych a,g,b
vpand ymm1, ymm3 ; wyzerowanie skladowych a,r,b
vpand ymm2, ymm3 ; wyzerowanie skladowych a,r,g

vpaddd ymm0, ymm1 ; zsumowanie skladowych r, g
vpaddd ymm0, ymm2 ; zsumowanie skladowych r, g, b

;dzielenie wartosci 3 kolejnych pikseli przez 3
vcvtdq2ps ymm1, ymm0 ; konwersja 32-bitowych liczb ca³kowitych na zmiennoprzecinkowe
vdivps ymm1, ymm4 ; dzielenie wektorowe
vcvtps2dq xmm0, xmm1 ; konwersja wektora liczb zmiennoprzecinkowych podwójnej/pojedynczej precyzji na 32-bitowy wektor liczby ca³kowitych

;wypelnienie rejestru ci¹gami 0, g, g, g gdzie g = (r + b + g) / 3
vmovaps ymm1, ymm0
vpslldq ymm0, 1
vpor ymm1, ymm0
vpslldq ymm0, 1

;przepisanie wartosci kanalu alpha
vpor ymm1, ymm0
vpor ymm1, ymm7

;wyslanie 4 pikseli do tablicy wynikowej
vmovdqu [edi], ymm1

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

ifbigger1
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
mov al, 255 ;ustawia max jeœli dodawanie przekroczy³oby max

next2:
mov [edi+2], al

add edi, 4
sub ecx, 3
loop startloop2


popad
rel
asmSepiaFilter endp

end