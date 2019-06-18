package com.kosta.kosta04;

import android.content.Intent;
import android.net.Uri;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

public class MainActivity extends AppCompatActivity
{
    Button btnNate, btn119, btnGallery, btnExit;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        btnNate = (Button)findViewById(R.id.btnNate);
        btn119 = (Button)findViewById(R.id.btn119);
        btnGallery = (Button)findViewById(R.id.btnGallery);
        btnExit = (Button)findViewById(R.id.btnExit);
        btnNate.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Intent mIntent = new Intent(Intent.ACTION_VIEW,Uri.parse("http://m.nate.com"));
                startActivity(mIntent);
            }
        });
    }
}
