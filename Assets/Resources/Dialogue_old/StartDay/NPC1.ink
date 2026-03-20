INCLUDE ../GlobalSettings/global.ink

#speaker:Крестьянин   
#portrait:black_man   
#layout:right
Пан, люди изнемогают от голода. Дозволь открыть амбары — отмерить горсть зерна каждой избе, пока поля не оживут.
#speaker:Крестьянин          
#portrait:black_man   
#layout:left
*   Насытить голодных.
-> accept
*   Отказать с насмешкой.
-> refuse
=== accept ===
Открою закрома, но чтоб ни зернышка не ушло мимо мерки. Скажешь «спасибо» — и марш в поле: весна сама хлеба не вырастит.
    #speaker:Крестьянин   
    #portrait:black_man   
    #layout:right
    Благодарность до небес, Пан! Народ отплатит потом урожаем и верностью.
    ~ Food -= 10
    ~ PeopleSatisfaction += 2
    -> END
=== refuse ===
Хочешь хлеб — копай глубже. Моё зерно не кормит ленивых ртов. Проваливай, пока стража не решила, что ты вор.  
    #speaker:Крестьянин   
    #portrait:black_man   
    #layout:right
    Понял, Пан… Народ запомнит этот приказ.
    ~ PeopleSatisfaction -= 2
    -> END
