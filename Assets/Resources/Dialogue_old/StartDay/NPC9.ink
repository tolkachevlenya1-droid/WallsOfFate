INCLUDE ../GlobalSettings/global.ink

#speaker:Слуга 
#portrait:black_woman 
#layout:right
Пан, каша в котлах горчит казённым дымом — специи вышли до крупинки. Кухарки ворчат, а солдаты плюют в кружки. Прикажете закупиться?
#speaker:Слуга 
#portrait:black_woman 
#layout:left
*  Освежить котлы.
-> accept
*  Потерпят без пряностей.
-> refuse

=== accept ===
Возьми из казны пригоршню монет, но смотри: каждая щепотка перца дороже солдатской головы. Вернёшься — отчитайся за каждый медяк.
#speaker:Слуга 
    #portrait:black_woman 
    #layout:right
Слушаюсь, пан. Кухня зазвенит ароматом, будто зал королевского пира.
    ~ Gold -= 10
    ~ PeopleSatisfaction += 1
    -> END

=== refuse ===
    На свадьбу пусть пряностей просят, а за стеной — война. Скажи кухаркам: соль и дым — лучшая приправа к мужнему духу.  
    #speaker:Слуга 
    #portrait:black_woman 
    #layout:right
    Будет так, пан… Только ворчание трудно унять.
    ~ PeopleSatisfaction -= 1
-> END