package com.example.kosta41recyclegrid;

public class Product
{
    String name;
    String manufacture;
    int countAgent;
    int price;
    int imageRes;

    public Product()
    {
    }

    public Product(String name,String manufacture,int countAgent,int price,int imageRes)
    {
        this.name=name;
        this.manufacture=manufacture;
        this.countAgent=countAgent;
        this.price=price;
        this.imageRes=imageRes;
    }

    public String getName()
    {
        return name;
    }

    public String getManufacture()
    {
        return manufacture;
    }

    public int getCountAgent()
    {
        return countAgent;
    }

    public int getPrice()
    {
        return price;
    }

    public int getImageRes()
    {
        return imageRes;
    }

    public void setName(String name)
    {
        this.name=name;
    }

    public void setManufacture(String manufacture)
    {
        this.manufacture=manufacture;
    }

    public void setCountAgent(int countAgent)
    {
        this.countAgent=countAgent;
    }

    public void setPrice(int price)
    {
        this.price=price;
    }

    public void setImageRes(int imageRes)
    {
        this.imageRes=imageRes;
    }
}
