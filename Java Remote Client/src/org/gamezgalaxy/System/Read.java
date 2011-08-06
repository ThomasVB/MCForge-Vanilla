package org.gamezgalaxy.System;
import java.util.Scanner;


public class Read implements Runnable {
	public Read() { }
	Scanner a = new Scanner(System.in);
	@Override
	public void run() {
		while (true)
			a.nextLine();
		
	}

}
