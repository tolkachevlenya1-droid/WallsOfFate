INCLUDE ../GlobalSettings/global.ink
# speaker: Гонец
# portrait: ms_yellow_neutral
# layout: right
Пан! Тяжёлая весть. Кто‑то выкрал из канцелярии все замковые отчёты: списки дозоров, запасы, схемы бойниц — пропало всё до последнего листа.
След ведёт к складу главного зала: на пыли отпечатки сапог и обрывок печати.

    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: left
*   Я разберусь.
    -> investigateWarehouse

*   Пусть займётся стража.
    -> dismissTheft

=== investigateWarehouse ===
    Бумаги с нашими слабостями не уйдут за ворота. Жди здесь — я прочешу склад, пока след тёплый.
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: right
Благодарю, пан. Если отчёты уйдут врагу — стены станут бумажными.
    -> END

=== dismissTheft ===
    До пояса дел. Передай капитану: пусть вытрясет крысу из каждой балки.
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: right
Как прикажете… Только вся стража уже надрывается, и вор успеет ускользнуть. Если бумаги попадут за пределы замка, кровь будет на наших руках.
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: left
    Ладно, чёрт с ним. Пока капитан соберётся, вор успеет выдувать сквозь щель.
    -> investigateWarehouse