package com.example.kosta13menu;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.graphics.Color;
import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.Button;
import android.widget.LinearLayout;

public class MainActivity extends AppCompatActivity {
    LinearLayout baseLayout;
    Button button;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        baseLayout = (LinearLayout)findViewById(R.id.baseLayout);
        button = (Button)findViewById(R.id.button);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        MenuInflater menuInflater = getMenuInflater();
        menuInflater.inflate(R.menu.option, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(@NonNull MenuItem item) {
        switch (item.getItemId())
        {
            case R.id.itemRed:
                baseLayout.setBackgroundColor(Color.RED);
                break;

            case R.id.itemGreen:
                baseLayout.setBackgroundColor(Color.GREEN);
                break;

            case R.id.itemBlue:
                baseLayout.setBackgroundColor(Color.BLUE);
                break;

            case R.id.subRotate:
                button.setRotation(45);
                break;

            case R.id.subSize:
                if(button.getScaleX()<2)
                {
                    button.setScaleX(2);
                    button.setScaleY(2);
                }
                else
                {
                    button.setScaleX(1);
                    button.setScaleY(1);
                }
                break;
        }
        return false;
    }
}
