package com.kosta.kosta05;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

public class SecondActivity extends AppCompatActivity
{
    Button btnReturnActivity;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_second);
        Intent mIntent = getIntent();
        final String Message = mIntent.getStringExtra("Message");
        final int Content = mIntent.getIntExtra("Content", 0);

        btnReturnActivity = (Button)findViewById(R.id.btnReturnActivity);
        btnReturnActivity.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Toast.makeText(getApplicationContext(), "받은 메시지" + Message + ":" + Integer.toString(Content), Toast.LENGTH_SHORT).show();
                finish();
            }
        });
    }
}
