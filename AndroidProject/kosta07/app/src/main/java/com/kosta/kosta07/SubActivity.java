package com.kosta.kosta07;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class SubActivity extends AppCompatActivity
{
    Button btnReturn;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sub);
        btnReturn = (Button)findViewById(R.id.btnReturn);
        final int hap = getIntent().getIntExtra("NUM1", 0) + getIntent().getIntExtra("NUM2", 0);

        btnReturn.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent outIntent = new Intent(getApplicationContext(), MainActivity.class);
                outIntent.putExtra("HAP", hap);
                setResult(RESULT_OK, outIntent);
                finish();
            }
        });
    }
}
