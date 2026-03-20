INCLUDE ../GlobalSettings/global.ink

#speaker:Слуга 
#portrait:black_woman 
#layout:right
Пан, дурные вести: хворь скосила караульных на северной башне. Лекарь клянёт, что отвар без хлеба — вода, просит удвоить им пайки, чтоб встали на ноги.
#speaker:Слуга 
#portrait:black_woman 
#layout:left
*   Дать хлеб и мясо.
-> accept
*   Никаких лишних мисок.
-> refuse

=== accept ===
    Забери припасы со склада, но считай каждую буханку. Солдат без силы — мёртвый камень на стене.
    #speaker:Слуга 
    #portrait:black_woman 
    #layout:right
    Будет исполнено, пан. Лекарь и бойцы не забудут вашей щедрости.
    ~ Food -= 10
    ~ CastleStrength += 30
    ~ PeopleSatisfaction += 1
    -> END

=== refuse ===
    Болезнь — испытание. Кто не выстоит на каше, не удержит и меча. Перебьются тем, что есть..
    #speaker:Слуга 
    #portrait:black_woman 
    #layout:right
    Поняла, пан. Сообщу лекарю… и подготовлю место в лазарете.
    ~ CastleStrength -= 20
    ~ PeopleSatisfaction -= 1
-> END