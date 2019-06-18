package com.kosta.kosta04;

import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.Toast;

public class CheckActivity extends AppCompatActivity
{
    Button btnCheckChange;
    CheckBox checkBox;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_check);
        btnCheckChange = (Button)findViewById(R.id.btnCheckChange);
        checkBox = (CheckBox)findViewById(R.id.checkBox);
        checkBox.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener()
        {
            @Override
            public void onCheckedChanged(CompoundButton buttonView,boolean isChecked)
            {
                Toast.makeText(CheckActivity.this, "Checked changed" + isChecked, Toast.LENGTH_SHORT).show();
            }
        });
        btnCheckChange.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View v)
            {
                checkBox.setChecked(checkBox.isChecked());

            }
        });
    }
}
