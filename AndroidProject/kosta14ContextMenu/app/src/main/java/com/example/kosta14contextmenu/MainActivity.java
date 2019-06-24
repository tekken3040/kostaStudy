package com.example.kosta14contextmenu;

import androidx.annotation.NonNull;
import androidx.appcompat.app.AppCompatActivity;

import android.graphics.Color;
import android.os.Bundle;
import android.view.ContextMenu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.view.View;
import android.widget.Button;
import android.widget.LinearLayout;

public class MainActivity extends AppCompatActivity {
    Button btn_BackgroundColor, btn_ButtonChange;
    LinearLayout baseLayout;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        btn_BackgroundColor = (Button)findViewById(R.id.btn_BackgroundColor);
        btn_ButtonChange = (Button)findViewById(R.id.btn_ButtonChange);
        baseLayout = (LinearLayout)findViewById(R.id.baseLayout);
        registerForContextMenu(btn_BackgroundColor);
        registerForContextMenu(btn_ButtonChange);
    }

    @Override
    public void onCreateContextMenu(ContextMenu menu, View v, ContextMenu.ContextMenuInfo menuInfo) {
        super.onCreateContextMenu(menu, v, menuInfo);
        MenuInflater menuInflater = getMenuInflater();
        if(v == btn_BackgroundColor)
        {
            menuInflater.inflate(R.menu.menu_background_color, menu);
        }
        else if(v == btn_ButtonChange)
        {
            menuInflater.inflate(R.menu.menu_button_change, menu);
        }
    }

    @Override
    public boolean onContextItemSelected(@NonNull MenuItem item) {
        super.onContextItemSelected(item);
        switch (item.getItemId())
        {
            case R.id.itemRed:
                baseLayout.setBackgroundColor(Color.RED);
                return true;

            case R.id.itemGreen:
                baseLayout.setBackgroundColor(Color.GREEN);
                return true;

            case R.id.itemBlue:
                baseLayout.setBackgroundColor(Color.BLUE);
                return true;

            case R.id.itemButtonSizeUp:
                if(btn_ButtonChange.getScaleX() < 2)
                {
                    btn_ButtonChange.setScaleX(2);
                    btn_ButtonChange.setScaleY(2);
                }
                else
                {
                    btn_ButtonChange.setScaleX(1);
                    btn_ButtonChange.setScaleY(1);
                }
                return true;

            case R.id.itemRotateButton:
                if(btn_ButtonChange.getRotation() < 45)
                {
                    btn_ButtonChange.setRotation(45);
                }
                else
                {
                    btn_ButtonChange.setRotation(0);
                }
                return true;
        }
        return false;
    }
}
