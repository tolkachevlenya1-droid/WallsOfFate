INCLUDE ../GlobalSettings/global.ink
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    (Разглядывая найденные улики)
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    1) Замковый отчёт цел, но страницы помяты: чужие пальцы лазили в мои секреты.  
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    2) Золотой кулон с моим гербом — исчез из кабинета неделю назад. 
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    3) Эти травы явно не отсюда. И среди моих подданных нет травницы.
# speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
Кто‑то шарил тут недавно — и знал, что ищет.
# speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    *   Изучить кулон подробнее
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
        -> analyzePendant

    *   Изучить мешочек
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
        -> followFlour

=== analyzePendant ===
# speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    Мой герб, выгравирован до линий. Значит, вор не только пробрался в склад, но и беспрепятственно гулял по моим покоям.
    Предатель близко.
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    -> connectClues

=== followFlour ===
# speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    Травинки тянутся тонкой змеёй вглубь подвала. Кто-то нарочно или по неосторожности раскидал их, будто метил дорогу к тайному выходу.    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    -> connectClues

=== connectClues ===
# speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
Кулон из моего кабинета. Экзотические травы, какие не найти в замковых садах. Шпион прячется среди своих — иначе как он прошёл стражу? Время играть в кошки‑мышки закончилось.
# speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
-> realization

=== realization ===
# speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
След из травинок ещё свежий. Прочешу склад прямо сейчас — крыса не уйдёт из своей норы.
# speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
-> END