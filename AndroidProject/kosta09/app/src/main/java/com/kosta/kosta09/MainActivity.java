package com.kosta.kosta09;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Color;
import android.graphics.Paint;
import android.os.Bundle;
import android.view.MotionEvent;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;

public class MainActivity extends AppCompatActivity
{
    MyGraphicView graphicView;
    final static int LINE=1, CIRCLE=2;
    static int currentShape = LINE;
    Button btnLine, btnCircle;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        LinearLayout canvasLayout = (LinearLayout)findViewById(R.id.canvasLayout);
        graphicView = new MyGraphicView(this);
        canvasLayout.addView(graphicView);
        btnLine = (Button)findViewById(R.id.btnLine);
        btnCircle = (Button)findViewById(R.id.btnCircle);
        btnLine.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                currentShape = LINE;
            }
        });
        btnCircle.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                currentShape = CIRCLE;
            }
        });
    }

    private static class MyGraphicView extends View
    {
        int startX = -1, startY = -1, stopX = -1, stopY = -1;
        @Override
        protected void onDraw(Canvas canvas)
        {
            super.onDraw(canvas);
            Paint paint = new Paint();
            paint.setAntiAlias(true);
            paint.setColor(Color.GREEN);
            paint.setStrokeWidth(5);
            paint.setStyle(Paint.Style.STROKE);

            switch(currentShape)
            {
                case LINE:
                    canvas.drawLine(startX, startY, stopX, stopY, paint);
                    break;

                case CIRCLE:
                    int radius = (int)Math.sqrt(Math.pow((stopX-startX), 2) + Math.pow((stopY-startY), 2));
                    canvas.drawCircle(startX, startY, radius, paint);
                    break;
            }
        }

        @Override
        public boolean onTouchEvent(MotionEvent event)
        {
            switch(event.getAction())
            {
                case MotionEvent.ACTION_DOWN:
                    startX = (int)event.getX();
                    startY = (int)event.getY();
                    invalidate();
                    break;

                case MotionEvent.ACTION_MOVE:
                case MotionEvent.ACTION_UP:
                    stopX = (int)event.getX();
                    stopY = (int)event.getY();
                    invalidate();
                    break;
            }
            return true;
        }

        public MyGraphicView(Context context)
        {
            super(context);
        }
    }
}
