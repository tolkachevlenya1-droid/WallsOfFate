INCLUDE ../GlobalSettings/global.ink

#speaker:Стражник 
#portrait:black_guard 
#layout:right
Пан! Воровская крыса сорвалась с цепи — умчала в лес с мешками провианта. Прикажете гнаться?
# speaker: Стражник
# portrait: black_guard
# layout: left
*   Догнать и на пику.
-> accept
*   Пусть лес его сожрёт.
-> refuse

=== accept ===
    Верни припасы и выставь его голову над воротами. Пусть каждый лиходей знает цену воровству.
    # speaker: Стражник
    # portrait: black_guard
    # layout: right
    Есть, пан. Отправлю быстрых, но знайте: бойцов придётсяотозвать со стен.
    ~ CastleStrength -= 20
    ~ PeopleSatisfaction += 1
    -> END

=== refuse ===
   Не растрачивай людей. Пускай волки примут его исповедь.
    # speaker: Стражник
    # portrait: black_guard
    # layout: right
    Как прикажете. Провиант не вернуть, но вороны насытятся.
    ~ Food -= 10
    -> END