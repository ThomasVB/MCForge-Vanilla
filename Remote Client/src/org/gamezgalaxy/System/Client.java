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
	public static Client main = new Client();
	public boolean startup;
	public boolean status;
	public String password;
	public String ip;
	public String port;
	TextField command;
	List chat;
	Button start_server;
	Button shutdown;
	Label Status;
	Label Command;
	static Socket socket = null;
	static PrintWriter out = null;
	static BufferedReader in = null;
	static boolean accepted = false;
	public Client() { }
	public void init()
	{
		this.setLayout(null);
		this.start_server = new Button("Start Server");
		this.command = new TextField(15);
		this.shutdown = new Button("Shutdown Server");
		//this.Chat = new Label("Live Chat");
		this.Command = new Label("Enter a server/client command");
		this.Status = new Label("Server Status: Standby..");
		this.chat = new List();
		this.start_server.setEnabled(false);
		this.shutdown.setEnabled(false);
		//this.chat.setEnabled(false);
		this.start_server.setBounds(0, 0, 120, 25);
		this.shutdown.setBounds(122, 0, 120, 25);
		this.chat.setBounds(0, 30, 242, 150);
		this.Command.setBounds(20, 200, 200, 10);
		this.command.setBounds(20, 225, 200, 10);
		this.Status.setBounds(0, 300, 200, 10);
		this.add(start_server);
		this.add(shutdown);
		this.add(chat);
		this.add(Command);
		this.add(command);
		this.add(Status);
		this.setSize(245, 320);
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
		main.chat.add(message);
	}
	public static void ServerMessage(String message)
	{
		main.chat.add("SERVER: " + message);
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
			out.print("START");
		else if (arg0.getSource() == shutdown && status)
			out.print("SHUTDOWN");
		
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
						//Thread a = new Thread(new Read());
						//a.start();
						Message("CONNECTED!");
						String line;
						while ((line = in.readLine()) != null)
						{
							ServerMessage(line);
							if (line.indexOf("STAY_UP") != -1)
								out.println("OK");
							else if (line.indexOf("UP") != -1)
							{
								status = true;
								this.Status.setText("Server Status: ONLINE");
							}
							else if (line.indexOf("DOWN") != -1)
							{
								status = false;
								this.Status.setText("Server Status: OFFLINE");
							}
							else if (line.indexOf("Accepted") != -1)
								accepted = true;
							else if (line.indexOf("Server_Chat:") != -1)
								Message(line);
							else if (line.indexOf("NO") != -1)
								System.exit(0);
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
			else
				out.print(command.getText());
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
