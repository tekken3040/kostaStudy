package com.kosta.kosta10miniphotoshop;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.graphics.Canvas;
import android.graphics.Paint;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.LinearLayout;

public class MainActivity extends AppCompatActivity
{
    ImageButton ibZoomIn, ibZoomOut, ibRotate, ibBright, ibDark, ibBlur, ibEmboss, ibGray;
    LinearLayout picLayout;
    static float scaleX = 1, scaleY = 1;
    static float angle = 0;
    static float saturation = 1;
    static float color = 1;
    static boolean blur = false;
    static boolean emboss = false;
    MyGraphicView myGraphicView;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        picLayout = (LinearLayout)findViewById(R.id.picLayout);
        myGraphicView = (MyGraphicView)new MyGraphicView(this);
        picLayout.addView(myGraphicView);
        clickIcons();
    }

    private void clickIcons()
    {
        ibZoomIn = (ImageButton)findViewById(R.id.ibZoomIn);
        ibZoomIn.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                scaleX += 0.2f;
                scaleY += 0.2f;
                myGraphicView.invalidate();
            }
        });

        ibZoomOut = (ImageButton)findViewById(R.id.ibZoomOut);
        ibZoomOut.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                scaleX -= 0.2f;
                scaleY -= 0.2f;
                myGraphicView.invalidate();
            }
        });

        ibRotate = (ImageButton)findViewById(R.id.ibRotate);
        ibRotate.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                angle += 20;
                myGraphicView.invalidate();
            }
        });

        ibBright = (ImageButton)findViewById(R.id.ibBright);
        ibBright.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                saturation += 0.2f;
                myGraphicView.invalidate();
            }
        });

        ibDark = (ImageButton)findViewById(R.id.ibDark);
        ibDark.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                saturation -= 0.2f;
                myGraphicView.invalidate();
            }
        });

        ibBlur = (ImageButton)findViewById(R.id.ibBlur);
        ibBlur.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                if(blur)
                    blur = false;
                else
                    blur = true;
                myGraphicView.invalidate();
            }
        });

        ibEmboss = (ImageButton)findViewById(R.id.ibEmboss);
        ibEmboss.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                if(emboss)
                    emboss = false;
                else
                    emboss = true;
                myGraphicView.invalidate();
            }
        });

        ibGray = (ImageButton)findViewById(R.id.ibGray);
        ibGray.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                if(saturation > 0)
                    saturation = 0;
                else
                    saturation = 1;
                myGraphicView.invalidate();
            }
        });
    }

    static class MyGraphicView extends View
    {
        public MyGraphicView(Context context)
        {
            super(context);
        }

        @Override
        protected void onDraw(Canvas canvas)
        {
            super.onDraw(canvas);
            int centerX = this.getWidth()/2;
            int centerY = this.getHeight()/2;
            canvas.scale(scaleX, scaleY, centerX, centerY);
            canvas.rotate(angle, centerX, centerY);

            Bitmap picture = BitmapFactory.decodeResource(getResources(), R.drawable.lena256);
            Paint paint = new Paint();
            canvas.drawBitmap(picture, this.getWidth() - picture.getWidth()/2, this.getHeight() - picture.getHeight()/2, paint);
            picture.recycle();
        }
    }
}
