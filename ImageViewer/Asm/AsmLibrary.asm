.data
.code
asmNegativeFilter proc
0F7904EE  xor         edx,edx  
0F7904F0  mov         dword ptr [ebp-50h],edx  
0F7904F3  nop  
0F7904F4  jmp         0F790553  
            {
0F7904F6  nop  

0F7904F7  mov         eax,dword ptr [ebp-50h]  
0F7904FA  mov         ecx,4  
0F7904FF  cdq  
0F790500  idiv        eax,ecx  
0F790502  cmp         edx,3  
0F790505  setne       al  
0F790508  movzx       eax,al  
0F79050B  mov         dword ptr [ebp-54h],eax  
0F79050E  cmp         dword ptr [ebp-54h],0  
0F790512  je          0F79054F  
                    pixels[i] =  (byte)(255 -pixels[i]);
0F790514  mov         eax,dword ptr [ebp-50h]  
0F790517  mov         edx,dword ptr [ebp-48h]  
0F79051A  cmp         eax,dword ptr [edx+4]  
0F79051D  jb          0F790524  
0F79051F  call        73683140  
0F790524  movzx       eax,byte ptr [edx+eax+8]  
0F790529  neg         eax  
0F79052B  add         eax,0FFh  
0F790530  and         eax,0FFh  
0F790535  mov         dword ptr [ebp-6Ch],eax  
0F790538  mov         eax,dword ptr [ebp-50h]  
0F79053B  mov         edx,dword ptr [ebp-48h]  
0F79053E  cmp         eax,dword ptr [edx+4]  
0F790541  jb          0F790548  
0F790543  call        73683140  
0F790548  mov         ecx,dword ptr [ebp-6Ch]  
0F79054B  mov         byte ptr [edx+eax+8],cl  
            }
0F79054F  nop  
            for (int i = 0; i < size; i++)
0F790550  inc         dword ptr [ebp-50h]  

0F790553  mov         eax,dword ptr [ebp-50h]  
0F790556  cmp         eax,dword ptr [ebp-44h]  
0F790559  setl        al  
0F79055C  movzx       eax,al  
0F79055F  mov         dword ptr [ebp-58h],eax  
0F790562  cmp         dword ptr [ebp-58h],0  
0F790566  jne         0F7904F6 
asmNegativeFilter endp

end