INCLUDE ../GlobalSettings/global.ink

#speaker:Бродяга 
#portrait:OldmenPortrait 
#layout:right
Пан, помилуйте… голодаю третий день. Поделитесь куском хлеба — и пускай небо благословит ваш дом.

#speaker:Бродяга 
#portrait:OldmenPortrait 
#layout:left
Сколько просишь и что дашь взамен?

#speaker:Бродяга 
#portrait:OldmenPortrait 
#layout:right
Имущества у меня — кот наплакал, но сохранил семейный медальон. 
Серебро потускнело, а память дорога. Отдам, лишь бы голод отстал.

#speaker:Бродяга 
#portrait:OldmenPortrait 
#layout:left
*  Накормить.
-> accept
*  Насмешка и изгнание.
-> refuse

=== accept ===
Держи паёк и воду. Медальон заберу в уплату — памятные вещи порой спасают крепость не хуже мечей.
#speaker:Бродяга 
#portrait:OldmenPortrait_Relieved 
#layout:right
    Спасибо, пан… Пусть ваши стены будут крепки, а хлеб никогда не черствеет.
    ~ Food -= 5
    ~ PeopleSatisfaction += 1
    ~ Gold += 10
    -> END

=== refuse ===
    Береги свой медальон, старик. Он пригодится, когда будешь платить Чёрту за место в канаве. 
    А теперь катись прочь, пока собаки не заметили лёгкую добычу.
#speaker:Бродяга 
#portrait:OldmenPortrait_Relieved 
#layout:right
    Понимаю… Прощайте, ваша милость.
    ~ PeopleSatisfaction -= 2 
    -> DONE