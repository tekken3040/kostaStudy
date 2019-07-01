package com.example.kosta39recycleview;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class MainActivity extends AppCompatActivity
{
    Layout layout;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        layout = findViewById(R.id.myLayout);
        layout.setImage(R.drawable.profile1);
        layout.setName("박명수");
        layout.setPhone("010-0000-1234");

        Button button1 = findViewById(R.id.button3);
        Button button2 = findViewById(R.id.button4);
        button1.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                layout.setImage(R.drawable.profile1);
            }
        });

        button2.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                layout.setImage(R.drawable.profile2);
            }
        });
    }
}
