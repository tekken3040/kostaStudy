package com.example.kosta41recyclegrid;

import androidx.appcompat.app.AppCompatActivity;
import androidx.recyclerview.widget.GridLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.os.Bundle;
import android.view.View;
import android.widget.Toast;

public class MainActivity extends AppCompatActivity
{
    RecyclerView recyclerView;
    ProductAdapter adapter;
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        recyclerView = findViewById(R.id.recyclerView);
        GridLayoutManager layoutManager = new GridLayoutManager(this, 2);
        recyclerView.setLayoutManager(layoutManager);

        adapter = new ProductAdapter();
        adapter.addItem(new Product("토끼털 코트", "리본", 7, 5478000, R.drawable.clothes1));
        adapter.addItem(new Product("어께 패치 H라인", "리본", 19, 6078000, R.drawable.clothes2));
        adapter.addItem(new Product("어께 패치 H라인", "리본", 1, 522000, R.drawable.clothes3));
        adapter.addItem(new Product("어께 패치 H라인", "리본", 10, 1278000, R.drawable.clothes4));
        adapter.addItem(new Product("어께 패치 H라인", "리본", 7, 308000, R.drawable.clothes5));
        adapter.addItem(new Product("어께 패치 H라인", "리본", 3, 78000, R.drawable.clothes6));
        recyclerView.setAdapter(adapter);
        adapter.setOnProductItemClickListener(new OnProductItemClickListener()
        {
            @Override
            public void onItemClick(ProductAdapter.ViewHolder holder,View view,int position)
            {
                Product item = (Product)adapter.getItem(position);
                Toast.makeText(getApplicationContext(), "선택된 제품 " + item.getName(), Toast.LENGTH_SHORT).show();
            }
        });
    }
}
