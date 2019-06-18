package com.kosta.practice;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity
{
    TextView textView1, textView2;
    CheckBox checkStart;
    LinearLayout selectionLayout;
    RadioGroup rGroup;
    RadioButton btnArcher, btnVestal, btnDocter;
    Button btnSelect;
    ImageView imageView;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        setTitle("직업 선택하기");

        textView1 = (TextView)findViewById(R.id.textView1);
        checkStart = (CheckBox)findViewById(R.id.checkStart);

        selectionLayout = (LinearLayout)findViewById(R.id.selectionLayout);
        textView2 = (TextView)findViewById(R.id.textView2);
        rGroup = (RadioGroup)findViewById(R.id.rGroup);
        btnArcher = (RadioButton)findViewById(R.id.btnArcher);
        btnVestal = (RadioButton)findViewById(R.id.btnVestal);
        btnDocter = (RadioButton)findViewById(R.id.btnDocter);
        btnSelect = (Button)findViewById(R.id.btnSelect);
        imageView = (ImageView)findViewById(R.id.imageView);

        checkStart.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener()
        {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton,boolean b)
            {
                if(checkStart.isChecked())
                    selectionLayout.setVisibility(View.VISIBLE);
                else
                    selectionLayout.setVisibility(View.INVISIBLE);
            }
        });

        btnSelect.setOnClickListener(new View.OnClickListener()
        {
            @Override
            public void onClick(View view)
            {
                switch(rGroup.getCheckedRadioButtonId())
                {
                    case R.id.btnArcher:
                        imageView.setImageResource(R.drawable.archer);
                        break;

                    case R.id.btnVestal:
                        imageView.setImageResource(R.drawable.vestal);
                        break;

                    case R.id.btnDocter:
                        imageView.setImageResource((R.drawable.plaguedoctor));
                        break;

                    default:
                        Toast.makeText(getApplicationContext(), "직업 먼저 선택하세요", Toast.LENGTH_SHORT).show();
                        break;
                }
            }
        });
    }
}
