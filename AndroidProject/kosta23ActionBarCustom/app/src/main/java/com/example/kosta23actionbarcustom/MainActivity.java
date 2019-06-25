package com.example.kosta23actionbarcustom;

import androidx.appcompat.app.ActionBar;
import androidx.appcompat.app.AppCompatActivity;

import android.graphics.Color;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity
{
    TextView textView1;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        textView1 = (TextView)findViewById(R.id.textView);
        ActionBar actionBar = getSupportActionBar();
        actionBar.setDisplayShowCustomEnabled(true);
        actionBar.setDisplayHomeAsUpEnabled(false);
        actionBar.setDisplayShowTitleEnabled(false);
        actionBar.setDisplayShowHomeEnabled(false);

        LayoutInflater layoutInflater = getLayoutInflater();
        View actionView = layoutInflater.inflate(R.layout.custom_actionbar, null);
        actionBar.setCustomView(actionView);

        Button button = (Button)actionView.findViewById(R.id.button);
        TextView textView2 = (TextView)actionView.findViewById(R.id.textView2);
        textView2.setText("커스텀 액션바");
        textView2.setTextColor(Color.WHITE);
        button.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                Toast toast = Toast.makeText(MainActivity.this, "버튼을 눌렀습니다", Toast.LENGTH_SHORT);
                toast.show();
            }
        });
    }
}
