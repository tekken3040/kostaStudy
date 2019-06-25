package com.example.kosta21actionview;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.Menu;
import android.view.MenuInflater;
import android.view.MenuItem;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.SearchView;
import android.widget.TextView;

public class MainActivity extends AppCompatActivity
{
    TextView textView1, textView2;
    ListView listView;
    SearchView searchView;
    String[] data_list = {"aaaa", "bbbb", "cccc", "dddd", "ccdd", "aabbc"};
    @Override
    protected void onCreate(Bundle savedInstanceState)
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        textView1 = (TextView)findViewById(R.id.textView1);
        textView2 = (TextView)findViewById(R.id.textView2);
        listView = (ListView)findViewById(R.id.listView);
        ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(this, android.R.layout.simple_expandable_list_item_1, data_list);
        listView.setAdapter(arrayAdapter);
        listView.setTextFilterEnabled(true);
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu)
    {
        MenuInflater menuInflater = getMenuInflater();
        menuInflater.inflate(R.menu.menu_main, menu);
        MenuItem menuItem = menu.findItem(R.id.item);
        menuItem.setOnActionExpandListener(new ActionListener());
        searchView = (SearchView)menuItem.getActionView();
        searchView.setOnQueryTextListener(new SearchView.OnQueryTextListener()
        {
            @Override
            public boolean onQueryTextSubmit(String s)
            {
                textView2.setText("엔터키를 눌렀습니다");
                return true;
            }

            @Override
            public boolean onQueryTextChange(String s)
            {
                textView2.setText(s);
                listView.setFilterText(s);
                if(s.length() == 0)
                    listView.clearTextFilter();
                return true;
            }
        });
        return true;
    }

    class ActionListener implements MenuItem.OnActionExpandListener
    {
        @Override
        public boolean onMenuItemActionExpand(MenuItem menuItem)
        {
            textView1.setText("펼쳐졌습니다");
            return true;
        }

        @Override
        public boolean onMenuItemActionCollapse(MenuItem menuItem)
        {
            textView1.setText("접혔습니다");
            return true;
        }
    }
}
