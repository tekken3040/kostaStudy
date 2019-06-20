package com.kosta.kosta11invader;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.util.DisplayMetrics;
import android.view.MotionEvent;
import android.view.View;

import java.util.ArrayList;
import java.util.Random;

public class MainActivity extends AppCompatActivity
{
    Bitmap screen, leftKey, rightKey, missileButton, spaceShip, missile, planet;
    int screenWidth, screenHeight;
    int spaceShipX, spaceShipY;
    int spaceShipWidth;
    int leftKeyX, leftKeyY;
    int rightKeyX, rightKeyY;
    int buttonWidth;
    int missileButtonX, missileButtonY;
    int missileWidth, missileMiddle;
    int score, count;
    ArrayList<Missile> missiles;
    ArrayList<Planet> planets;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(new MyView(this));
        //setContentView(R.layout.activity_main);
        DisplayMetrics displayMetrics = getApplicationContext().getResources().getDisplayMetrics();
        planets = new ArrayList<Planet>();
        missiles = new ArrayList<Missile>();

        screenWidth = displayMetrics.widthPixels;
        screenHeight = displayMetrics.heightPixels;
        //스크린====================================================================================
        screen = BitmapFactory.decodeResource(getResources(), R.drawable.screen0);
        screen = Bitmap.createScaledBitmap(screen, screenWidth, screenHeight, true);
        //우주선====================================================================================
        spaceShip = BitmapFactory.decodeResource(getResources(), R.drawable.spaceship);
        spaceShipX = screenWidth/9;
        spaceShipY = screenHeight*6/9;
        int x = screenWidth/7, y = screenHeight/11;
        spaceShip = Bitmap.createScaledBitmap(spaceShip, x, y, true);
        spaceShipWidth = spaceShip.getWidth();
        //왼쪽키====================================================================================
        leftKey = BitmapFactory.decodeResource(getResources(), R.drawable.leftkey);
        leftKeyX = screenWidth/11;
        leftKeyY = screenHeight*7/9;
        buttonWidth = screenWidth/6;
        leftKey = Bitmap.createScaledBitmap(leftKey, buttonWidth, buttonWidth, true);
        //오른쪽키==================================================================================
        rightKey = BitmapFactory.decodeResource(getResources(), R.drawable.rightkey);
        rightKeyX = screenWidth/2;
        rightKeyY = screenHeight*7/9;
        buttonWidth = screenWidth/6;
        rightKey = Bitmap.createScaledBitmap(rightKey, buttonWidth, buttonWidth, true);
        //미사일키==================================================================================
        missileButton = BitmapFactory.decodeResource(getResources(), R.drawable.missilebutton);
        missileButtonX = screenWidth/3;
        missileButtonY = screenHeight*7/9;
        buttonWidth = screenWidth/6;
        missileButton = Bitmap.createScaledBitmap(missileButton, buttonWidth, buttonWidth, true);
        //미사일====================================================================================
        missile = BitmapFactory.decodeResource(getResources(), R.drawable.missile0);
        missile = Bitmap.createScaledBitmap(missile, buttonWidth/4, buttonWidth/4, true);
        missileMiddle = buttonWidth/8;
        missileWidth = buttonWidth/4;
        //행성======================================================================================
        planet = BitmapFactory.decodeResource(getResources(), R.drawable.planet);
        planet = Bitmap.createScaledBitmap(planet, buttonWidth, buttonWidth, true);
    }

    class MyView extends View
    {
        Handler gHandler = new Handler()
        {
            @Override
            public void handleMessage(@NonNull Message msg)
            {
                invalidate();
                gHandler.sendEmptyMessageDelayed(0, 500);
            }
        };
        public MyView(Context context)
        {
            super(context);
            gHandler.sendEmptyMessageDelayed(0, 500);
        }

        public void moveMissiles()
        {
            if(missiles.size() > 0)
            {
                for(int i=missiles.size()-1;i >= 0;i--)
                {
                    missiles.get(i).moveUp();
                    if(missiles.get(i).y<0)
                        missiles.remove(i);
                }
            }
        }

        public void movePlanets()
        {
            if(planets.size() > 0)
            {
                for(int i=planets.size()-1;i >= 0;i--)
                {
                    planets.get(i).moveDown();
                    if(planets.get(i).y>screenHeight)
                        planets.remove(i);
                }
            }
        }

        public void collectionDetect()
        {
            for(int i=planets.size()-1; i>=0; i--)
            {
                for(int j=missiles.size()-1; j>=0; j--)
                {
                    if(missiles.get(j).x+missileMiddle>planets.get(i).x
                    && missiles.get(j).x+missileMiddle<planets.get(i).x+buttonWidth
                    && missiles.get(j).y+missileMiddle>planets.get(i).y
                    && missiles.get(j).y+missileMiddle<planets.get(i).y+buttonWidth)
                    {
                        planets.remove(i);
                        missiles.get(j).y = -30;
                        score += 10;
                    }
                }
            }
        }

        @Override
        protected void onDraw(Canvas canvas)
        {
            super.onDraw(canvas);
            Random random = new Random();
            int planetX = random.nextInt(screenWidth);
            if(planets.size() < 5)
                planets.add(new Planet(planetX, 100));
            Paint paint = new Paint();
            canvas.drawBitmap(screen, 0, 0, paint);
            canvas.drawBitmap(spaceShip, spaceShipX, spaceShipY, paint);
            canvas.drawBitmap(leftKey, leftKeyX, leftKeyY, paint);
            canvas.drawBitmap(rightKey, rightKeyX, rightKeyY, paint);
            canvas.drawBitmap(missileButton, missileButtonX, missileButtonY, paint);

            for(Planet ele : planets)
                canvas.drawBitmap(planet, ele.x, ele.y, paint);
            for(Missile ele : missiles)
                canvas.drawBitmap(missile, ele.x, ele.y, paint);

            moveMissiles();
            movePlanets();
            collectionDetect();
        }

        @Override
        public boolean onTouchEvent(MotionEvent event)
        {
            int x = 0, y = 0;
            if(event.getAction() == MotionEvent.ACTION_DOWN || event.getAction() == MotionEvent.ACTION_MOVE)
            {
                x = (int)event.getX();
                y = (int)event.getY();
                invalidate();
            }

            if(x>leftKeyX && x<leftKeyX+buttonWidth && y>leftKeyY && y<leftKeyY+buttonWidth)
                spaceShipX -= 20;
            if(x>rightKeyX && x<rightKeyX+buttonWidth && y>rightKeyY && y<rightKeyY+buttonWidth)
                spaceShipX += 20;

            if(x>missileButtonX && x<missileButtonX+buttonWidth && y>missileButtonY && y<missileButtonY+buttonWidth)
            {
                if(missiles.size() < 10)
                    missiles.add(new Missile(spaceShipX+spaceShipWidth/2 - missileWidth/2, spaceShipY));
            }

            return true;
        }
    }
}
