INCLUDE ../GlobalSettings/global.ink

#speaker:Музыкантка 
#portrait:black_woman 
#layout:right
Милостивый пан, дай нам кров и скудный ужин. Взамен разгоним страх в сердцах — песней о доблести и грядущей победе.
#speaker:Музыкантка 
#portrait:black_woman 
#layout:left
*   Пусть поют.
-> accept
*   Не до музыки.
-> refuse

=== accept ===
    Бери яства из кухни, но считай каждый каравай. Когда струны запоют — пусть песня станет бронёй, где не достаёт стали.
    #speaker:Музыкантка 
#portrait:black_woman 
#layout:right
    Светлая благодарность, пан. Ваша щедрость прозвучит до последнего бастиона.
    ~ Food -= 5
    ~ PeopleSatisfaction += 2
    ~CastleStrength += 10
    -> END

=== refuse ===
    Здесь пахнет гарью битвы, не пиром. Лютня не остановит стрелу. Ищи крышу вдали от моих стен.
    #speaker:Музыкантка 
    #portrait:black_woman 
    #layout:right
    Понимаю… Но вспомните: порой песня крепче брони. Прощайте, пан.
    ~ PeopleSatisfaction -= 1
    -> END