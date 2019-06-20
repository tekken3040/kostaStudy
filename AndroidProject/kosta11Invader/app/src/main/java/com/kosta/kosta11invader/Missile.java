package com.kosta.kosta11invader;

public class Missile
{
    int x, y;
    int missileSpeed = 35;

    Missile(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void moveUp()
    {
        y -= missileSpeed;
    }
}
