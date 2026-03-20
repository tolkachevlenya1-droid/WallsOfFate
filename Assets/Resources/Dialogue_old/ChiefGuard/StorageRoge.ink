INCLUDE ../GlobalSettings/global.ink
# speaker: Стражник
# portrait: GuardPortrait
# layout: left
Стража! В зерновом амбаре прижал воришку — трясётся меж мешков, как крыса под метлой.
# speaker: Стражник
# portrait: GuardPortrait
# layout: right
Слушаю, пан. Что делаем с плутом?
# speaker: Стражник
# portrait: GuardPortrait
# layout: left
*   Морить голодом.
-> accept
*   Выбить правду.
-> refuse

=== accept ===
    Запри его в подвалы. Ни крошки, ни глотка воды три дня. Пусть узнает цену чужого зерна.
    # speaker: Стражник
    # portrait: GuardPortrait
    # layout: right
    Есть, ваша светлость. Три дня без пищи — и он сам себя сдаст.
    ~ Food += 15
    ~ PeopleSatisfaction += 1
    -> END

=== refuse ===
    Свяжи крепче и в пыточную. Хочу имена сообщников и схроны — выверни его, пока не заговорит.
    # speaker: Стражник
    # portrait: GuardPortrait
    # layout: right
    Будет исполнено. Подвешу к балке — к петухам вытрясу каждую крупицу правды.
    ~ PeopleSatisfaction += 1
    ~ CastleStrength += 50
    -> END
