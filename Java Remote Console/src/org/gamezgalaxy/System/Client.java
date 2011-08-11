package org.gamezgalaxy.System;

import java.applet.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.Scanner;



public class Client extends Applet implements ActionListener, KeyListener {
	public boolean startup;
	public boolean status;
	public String password;
	public String ip;
	public String port;
	TextField command;
	List chat;
	Button start_server;
	Button shutdown;
	Label Chat;
	Label Command;
	static Socket socket = null;
	static PrintWriter out = null;
	static BufferedReader in = null;
	static boolean accepted = false;
	public void init()
	{
		this.start_server = new Button("Start Server");
		this.command = new TextField(15);
		this.shutdown = new Button("Shutdown Server");
		//this.Chat = new Label("Live Chat");
		this.Command = new Label("Enter a server/client command");
		this.chat = new List();
		this.start_server.setEnabled(false);
		this.shutdown.setEnabled(false);
		//this.chat.setEnabled(false);
		this.add(start_server);
		this.add(shutdown);
		this.add(Command);
		this.add(command);
		//this.add(Chat);
		this.add(chat);
		this.chat.setBounds(command.getX(), command.getY() - 10, 200, 200);
		this.chat.add("LIVE CHAT");
		this.chat.add("Please type in the password for your server");
		this.chat.add("In the command box in hit enter");
		this.chat.addActionListener(this);
		this.command.addKeyListener(this);
		this.start_server.addActionListener(this);
		this.shutdown.addActionListener(this);

	}
	public static void Message(String message)
	{
		
	}
	public static void ServerMessage(String message)
	{
		System.out.print("SERVER: " + message + "\n");
	}
	public static void Sleep(int seconds)
	{
		try {
			Thread.sleep(seconds);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	@Override
	public void actionPerformed(ActionEvent arg0) {
		if (arg0.getSource() == start_server && !status)
		{
			out.print("START");
		}
		
	}
	@Override
	public void keyPressed(KeyEvent arg0) {
		if (arg0.getSource() == command && arg0.getKeyCode() == KeyEvent.VK_ENTER)
		{
			if (startup && password.equals(""))
			{
				password = command.getText();
				command.setText("");
				this.chat.add("Now enter the IP of your server");
			}
			else if (startup && ip.equals(""))
			{
				ip = command.getText();
				command.setText("");
				this.chat.add("Now enter the port of the remote console.");
			}
			else if (startup && port.equals(""))
			{
				try{
					socket = new Socket(ip, Integer.parseInt(port));
					out = new PrintWriter(socket.getOutputStream(), true);
					in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
					out.println("PASSWORD " + password);
				} catch (UnknownHostException e) {
					System.out.println("Unknown host: " + ip);
				} catch  (IOException e) {
					System.out.println("No I/O");
				}
				if (socket != null && out != null && in != null)
				{
					try 
					{
						Thread a = new Thread(new Read());
						a.start();
						Message("CONNECTED!");
						String line;
						while ((line = in.readLine()) != null)
						{
							ServerMessage(line);
							if (line.indexOf("STAY_UP") != -1)
								out.println("OK");
							else if (line.indexOf("UP") != -1)
								status = true;
							else if (line.indexOf("DOWN") != -1)
								status = false;
							else if (line.indexOf("Accepted") != -1)
								accepted = true;
							if (line.indexOf("KILL") != -1)
								break;
							if (status)
							{
								start_server.setEnabled(false);
								shutdown.setEnabled(true);
							}
							else
							{
								start_server.setEnabled(true);
								shutdown.setEnabled(false);
							}
							Thread.sleep(200);
						}
						out.close();
						in.close();
						socket.close();
					}
					catch (Exception e) { 
						Message("ERROR!");
						Message(e.toString());
					}
				}
				ServerMessage("DONE!");
			}
		}
		
	}
	@Override
	public void keyReleased(KeyEvent arg0) {
		// TODO Auto-generated method stub
		
	}
	@Override
	public void keyTyped(KeyEvent arg0) {
		// TODO Auto-generated method stub
		
	}

}
