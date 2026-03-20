INCLUDE ../GlobalSettings/global.ink

#speaker:Стражник 
#portrait:black_guard 
#layout:right
Пан, схватили шайку лесных волков — пятеро, руки ещё липнут от чужой крови и хлеба. Как укажете распорядиться тварями?
#speaker:Стражник 
#portrait:black_guard 
#layout:left
*   Виселица у ворот.
-> accept
*   Пусть искупят кровью.
-> refuse

=== accept ===
    Повесьте высоко, чтоб ветер трепал, а каждый зевак считал кости. Пост и страх — лучшая проповедь для прочих разбойников.
    #speaker:Стражник 
    #portrait:black_guard 
    #layout:right
    Сделаем, пан. На заре они встретят петлю.
    ~ PeopleSatisfaction += 1
    -> END

=== refuse ===
    Дайте им железо и цепи. Будут рубить врага в авангарде: шаг назад — арбалетный болт в спину. Так кровь смоет их грех.
    #speaker:Стражник 
    #portrait:black_guard 
    #layout:right
    Понял. Свяжем им судьбу кандалами и шансов не дадим.
    ~ CastleStrength += 20
-> END