INCLUDE ../GlobalSettings/global.ink
# speaker: Бродяга
# portrait: OldmenPortrait
# layout: right
П‑пожалуйста, пан… Не выдавайте меня. Я пережду войну в тени и уйду, как будто меня тут не было.

# speaker: Бродяга
# portrait: OldmenPortrait
# layout: left
Слышал я о волке, что прикинулся овцой. Говорят, звался Голован, генерал, что держал границу годами. Так ли это, "дед"?

# speaker: Бродяга
# portrait: OldmenPortrait
# layout: right
Было время… теперь я никому не нужен. Оставьте меня в покое.

# speaker: Бродяга
# portrait: OldmenPortrait
# layout: left
*   Стань под моё знамя.
    -> gate1RefugeeMainRoom

*   Силой вытащить на службу.
    -> gate2RefugeeMainRoom

=== gate1RefugeeMainRoom ===
    В покое? Пока ты прячешься, мои люди льют кровь. Поднимайся, Волк. Дам хлеб, доспех и власть — взамен хочу твоего гения на стенах.
    # speaker: Бродяга
    # portrait: OldmenPortrait
    # layout: right
    Хлеб и доспех я приму. Но власть… она тяжела.
    Ладно. Ради тех, кто ещё верит в эту землю, я вновь возьмусь за сталь.
    ~ CastleStrength += 30
    -> END

=== gate2RefugeeMainRoom ===
    Мне не нужны отговорки. Либо ты ведёшь людей, либо кормишь ворон. Выбирай.
    # speaker: Бродяга
    # portrait: OldmenPortrait
    # layout: right
    Так вот какая плата за тишину… 
    Придётся напомнить тебе, что Волк не сдаётся без укуса.
  ~ PowerCheckStart = true
    -> END