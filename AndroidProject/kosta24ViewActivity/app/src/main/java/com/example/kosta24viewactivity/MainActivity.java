package com.example.kosta24viewactivity;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.TextView;
import android.widget.Toolbar;

public class MainActivity extends AppCompatActivity
{
    androidx.appcompat.widget.Toolbar toolbar;
    TextView textView;

    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        toolbar = (androidx.appcompat.widget.Toolbar)findViewById(R.id.toolbar);
        textView = (TextView)findViewById(R.id.textView);

        setSupportActionBar(toolbar);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        MenuInflater menuInflater = getMenuInflater();
        menuInflater.inflate(R.menu.main_menu, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(@NonNull MenuItem item)
    {
        switch(item.getItemId())
        {
        case R.id.item1:
            textView.setText("메뉴 1번 눌렀습니다.");
            break;

        case R.id.item2:
            textView.setText("메뉴 2번 눌렀습니다.");
            break;

        case R.id.item3:
            textView.setText("메뉴 3번 눌렀습니다.");
            break;
        }
        return super.onOptionsItemSelected(item);
    }
}
