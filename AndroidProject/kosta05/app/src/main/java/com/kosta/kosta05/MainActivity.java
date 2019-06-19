package com.kosta.kosta05;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class MainActivity extends AppCompatActivity
{
    Button btnNewActivity;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        btnNewActivity = (Button)findViewById(R.id.btnNewActivity);
        btnNewActivity.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent mIntent = new Intent(MainActivity.this, SecondActivity.class);
                mIntent.putExtra("Message", "연습");
                mIntent.putExtra("Content", 100);
                startActivity(mIntent);
            }
        });
    }
}
