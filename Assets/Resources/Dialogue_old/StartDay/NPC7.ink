INCLUDE ../GlobalSettings/global.ink

#speaker:Кочевник 
#portrait:black_man 
#layout:right
Пан, долгая дорога истощила все мои запасы. Продай горсть провианта — золото в руке.
#speaker:Кочевник 
#portrait:black_man 
#layout:left
*   Монету на стол — и получишь хлеб.
-> accept
*   Замок — не лавка.
-> refuse

=== accept ===
    Золото вперёд. Получишь хлеб и сушёное мясо, а потом катись по ветру.
   #speaker:Кочевник 
    #portrait:black_man 
    #layout:right
    Благодарю, пан. Пускай твоя казна звенит громче врага.
    ~ Food -= 15
    ~ Gold += 20
    -> END

=== refuse ===
Самим не хватает. Замок кормит тех, кто его защищает. Разыщи харч на трактах, как прочие кочевники.
    #speaker:Кочевник 
    #portrait:black_man 
    #layout:right
    Пустой желудок ходит тёмными тропами, пан. Береги запасы.
    ~ Food -= 5
-> END