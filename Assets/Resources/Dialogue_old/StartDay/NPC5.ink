INCLUDE ../GlobalSettings/global.ink

#speaker:Странник 
#portrait:black_man 
#layout:right
Пан, у меня есть слухи, что стоят звонкой монеты. Клады, маршруты врага, трещины в их броне… Интересует?
#speaker:Странник 
#portrait:black_man 
#layout:left
*   Заплати за тайну.
-> accept
*   Не кормлю сплетников.
-> refuse

=== accept ===
    Монету дам, но если слова твои пусты — заплатишь языком. Говори.
    #speaker:Странник 
    #portrait:black_man 
    #layout:right
    Дорога через старый овраг размыта — войско врага двинется в обход, прямо под западный склон. Устройте там засаду — и перережете им хвост.
~ Gold -= 10
~ CastleStrength += 30
    -> END

=== refuse ===
    Чужие секреты пахнут тухлой рыбой. Храни их при себе.
    #speaker:Странник 
    #portrait:black_man 
    #layout:right
    Как желаете, пан. Но рыба, что гниёт в тени, порой губит целый пир.
    -> END