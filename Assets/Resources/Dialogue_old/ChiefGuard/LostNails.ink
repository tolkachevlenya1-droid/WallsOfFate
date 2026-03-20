INCLUDE ../GlobalSettings/global.ink
# speaker: Глава стражи
# portrait: GuardPortrait
# layout: right
Пан, ждём приказа. Куда направить клинки?

    # speaker: Глава стражи
    # portrait: GuardPortrait
    # layout: left
*   Допросить кузнеца.
    -> interrogateBlacksmith
*   Арестовать бродягу.
    -> arrestBeggar

=== interrogateBlacksmith ===
    В кузне тают гвозди. Возьми пару крепких парней, тряхни мастера: пусть железо градом сыплется — правду вытрясите.
    # speaker: Глава стражи
    # portrait: GuardPortrait
    # layout: right
    По‑вашему будет. Загляну в горн и в кошель мастера. Если врёт — не позавидуешь ему.   
    ~ PeopleSatisfaction -= 1
    ~ CastleStrength -= 40
    -> END

=== arrestBeggar ===
    Кузнец клянёт оборванца у ворот. Свяжи нищего, выведай, где тайник, и пригрози кандалами, коли замолчит.
    # speaker: Глава стражи
    # portrait: GuardPortraitn
    # layout: right
    Есть, пан. Скрутим, прижмём к стенке. Если унес гвозди — пойдёт в ополчение без права отказа.    
    ~ PeopleSatisfaction -= 1
    ~ CastleStrength += 10
    -> END