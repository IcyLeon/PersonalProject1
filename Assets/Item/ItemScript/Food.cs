using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Food : ConsumableItem
{
    FoodData fooddata;
    public override string GetItemType()
    {
        return "Food";
    }

    public override void Use(int Useamount)
    {
        base.Use(Useamount);
    }
    public Food(FoodData fd) : base()
    {
        category = Category.FOOD;
        item = fooddata.itemTemplates;
        fooddata = fd;
    }
}
