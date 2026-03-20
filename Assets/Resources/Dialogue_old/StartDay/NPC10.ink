INCLUDE ../GlobalSettings/global.ink

#speaker:Слуга 
#portrait:black_man 
#layout:right
Пан, шторм сорвал дранку: в крыше склада дыра с кулак. Если не латать — дождь сгноит зерно и солдаты будут глодать плесень.
#speaker:Слуга 
#portrait:black_man 
#layout:left
*   Залатать немедля.
-> accept
*   Обойдёмся.
-> refuse

=== accept ===
    Возьми мастеров и тёзки медяков. Если хоть один мешок заплесневеет — ты же вместо досок крышу собой накроешь.
    #speaker:Слуга 
    #portrait:black_man 
    #layout:right
    Сделаем, пан. К заходу солнца крыша будет крепче доспеха.
    ~ Gold -= 20
    -> END

=== refuse ===
Нет лишнего золота. Расстелите брезент — а кто станет ныть о сырости, пусть жуёт зерно сырым.    
#speaker:Слуга 
    #portrait:black_man 
    #layout:right
Как пожелаете… Но плесень не щадит животы, пан.
~ Food -= 20
-> END