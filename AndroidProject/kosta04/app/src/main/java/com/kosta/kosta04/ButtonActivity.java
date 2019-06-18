package com.kosta.kosta04;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.Toast;

public class ButtonActivity extends AppCompatActivity
{
    Button btnTop, btnBelow;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_button);
        btnTop = (Button)findViewById(R.id.btnTop);
        btnBelow = (Button)findViewById(R.id.btnBelow);
        btnTop.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Toast.makeText(ButtonActivity.this, "위에 버튼을 누름", Toast.LENGTH_SHORT).show();
            }
        });
    }

    public void OnButtonClick(View view)
    {
        Toast.makeText(ButtonActivity.this, "아래 버튼을 누름", Toast.LENGTH_SHORT).show();
    }
}
