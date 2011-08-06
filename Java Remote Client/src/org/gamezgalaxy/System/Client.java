package org.gamezgalaxy.System;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.PrintWriter;
import java.net.Socket;
import java.net.UnknownHostException;
import java.util.Scanner;


public class Client implements ActionListener{
	static Scanner a = new Scanner(System.in);
	static Socket socket = null;
	static PrintWriter out = null;
	static BufferedReader in = null;
	public static void main(String[] args)
	{
		Message("Type in the password");
		String pass = a.nextLine();
		Message("Type in the ip of the remote console you want to connect to..");
		String server = a.nextLine();
		Message("Type in the port of the remote console");
		int port = a.nextInt();
		Message("Attempting to connect...");
		try{
			socket = new Socket(server, port);
			out = new PrintWriter(socket.getOutputStream(), true);
			in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
			out.println("PASSWORD " + pass);
		} catch (UnknownHostException e) {
			System.out.println("Unknown host: " + server);
		} catch  (IOException e) {
			System.out.println("No I/O");
		}
		Thread a = new Thread(new Read());
		a.start();
		Message("CONNECTED!");

	}
	public static void Message(String message)
	{
		System.out.print("SYSTEM: " + message + "\n");
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
		try
		{
			String line = in.readLine();
			ServerMessage(line);
		}
		catch (IOException e)
		{
			Message("READ FAILED!");
		}
		
	}

}
