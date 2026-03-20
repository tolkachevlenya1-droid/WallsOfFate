INCLUDE ../GlobalSettings/global.ink
# speaker: Стражник
# portrait: black_guard
# layout: right
Пан! Клинок ржавеет без дела. Не желаете ли скрестить стали — ради тренировки, разумеется?

    # speaker: Стражник
    # portrait: black_guard
    # layout: left
*   Ну, ты нарвался...
    -> gate1Guard

*   У меня нет время на игры.
    -> gate2Guard

=== gate1Guard ===
    Удовлетворю твою прыть. Становись, проверим, насколько твёрды твои рёбра.
    # speaker: Стражник
    # portrait: black_guard
    # layout: right
    С радостью, пан! Без оголтелой злобы, но до первой крови — чтоб рука помнила резь.   
    ~ CastleStrength += 1
     ~ PowerCheckStart = true
    -> END

=== gate2Guard ===
    Отставить. Дела важней, чем махать железом ради забавы.
    # speaker: Стражник
    # portrait: black_guard
    # layout: right
    Как прикажете. Клинок будет ждать, когда у вас найдётся минута.
    -> END