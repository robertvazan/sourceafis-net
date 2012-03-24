package simple;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import sourceafis.simple.AfisEngine;
import sourceafis.simple.Fingerprint;
import sourceafis.simple.Person; 
import org.apache.commons.codec.binary.Base64;

public class AFISExample {
	public static void main(String[] args) throws IOException {
		
		String t1="Rk1SACAyMAAAAAE4AAABYAIgAMUAxQEAAAAAL0CaATHcAECPAUPJAEC9AWm1AECTAPP/AECSAXQ1AEDJAYCpAEBmAPYVAEC/AM/yAEBNAVDEAEEiAU7VAEBCAWhBAECfALABAEE2AQzlAECvAcgOAECtAHqGAEC0AGf6AEDKATrSAECbAQ3yAEByAT4bAEDZAWm/AEBjAT7DAECwAYwuAED6AXLLAEB3AYQGAEEeATPbAEA+AVXGAEEsAUVbAIDOAbchAIDFAKZ8AICSAdZ6AID3AIZvAICIAGcJAIDMAR/cAICtAWGyAIDWAPzoAID1AUrVAICYAXyGAIDqAXi9AIDiANxwAIBhAPKhAIC1AaIkAID0AZa/AIA2ARGmAIDpAa8VAICMAKEJAICcAeUHAIBwAH2VAAAA";
		String t2="Rk1SACAyMAAAAAFKAAABYAIgAMUAxQEAAAAAMkC/ASbVAECgAUq1AECBATCzAECRAPz1AEDeAVvAAEBlAS4cAECJAN4AAEBtAW0BAEDpAXy1AEDaAMh1AEDDAZwhAEDfAZUsAECRAKABAEEDAaBwAECDAcV/AEDqAHhwAECjAF3/AEDBAQveAEDGAUzEAECMAUifAEDrATXVAEB4AVROAECiAXUuAECBAXF6AECoAYkhAEEUAT7YAEBaAOoVAEElAS9hAEEoAQHmAEEeAYBSAIB9AJgLAICaAdILAICdAGqJAIB3AGELAICOAR/eAICwAVSyAICPAQP1AIDCAWqmAIDOAOXoAIDvAVjLAIBVATLBAIEUASDfAIC0AMDzAIA+AUPBAIBIAO2kAICjAa8OAIC2AJiAAIEZAaNeAICEAdkDAIBcAHObAAAA";
		String t3="Rk1SACAyMAAAAAE+AAABYAIgAMUAxQEAAAAAMEDDASPSAEDJAQPeAEC3AU2yAEBtASQaAEDkAVTAAECTANYAAED6AVDLAEBkAOEVAEDvAXiyAEEgAR7YAEDjAZAoAEA3AUHHAEEdAYBNAEEIAZxzAECIAdQBAEBuAGmYAECWARfcAECJASqyAEDSAUTBAEDzAS/VAECpAW4sAEBeASnAAEByAWX/AEDkAMVyAEBHATq/AEEeATrUAEBAAVREAECpAasQAIDGAJB/AICHAb9/AID7AHJvAIC0AFT9AICnAUOzAICUAT+kAICbAPTzAIDIAWWlAIDaAN7mAICIAWp9AICvAYIhAIDBALjwAIBUAOKiAIDHAZghAIEsATBYAICgAJb/AICOAI4JAICfAccLAICuAGGJAICHAFcKAAAA";
		String t4="Rk1SACAyMAAAAAE+AAABYAIgAMUAxQEAAAAAMEDDASrSAECjAU21AEC0AVewAEDvATnVAEBoAS8aAECPAOL/AEBZATPBAECrAYwhAEDsAYC/AEBBAUTBAEDFAaEhAECXAKQBAEC+AJx/AEEFAaR1AECGAdcEAEBpAHmVAECSASPcAECDAS6uAECPAUymAEDQAO/pAEDhAWC/AECEAXR5AEBuAW//AEDbANFyAEEaASjaAIBPAO2hAIDhAZkaAIEyAQPrAIEfAYRQAICDAcl8AIDxAH1wAICqAGD8AIDEARDfAICUAQf1AIDKAVLBAIC/AWyoAIClAXgsAIDzAV/LAIC6AMr1AIBeAO0UAIEYAUPXAIEmATlaAIAtAU3JAICkAbMOAICDAJwLAICcAdALAICjAG2JAIB+AGQLAAAA";
		
		String probeTemplate="Rk1SACAyMAAAAAFQAAABYAIgAMUAxQEAAAAAM0DHASnSAECQASyuAECXAUacAEDYAU6/AEDMAWmmAECsAXQsAECKAW96AECyAYghAEDFALzwAEEiASLaAEDJAZ0eAEA7AUbDAEAvAQWpAEDKAJR/AEEMAaNuAECKAdgAAEByAHCXAEDLARtzAECyAUqwAEC7AVSvAEByASkaAEDoAVu/AECXANr/AED6AVrKAEBpAOQSAEDyAX2/AEBZAOWhAEDmAZYXAEA+AVlAAECqAa8OAEElAYNOAECIAcN9AICwAGqHAICOAF8LAICbARzcAIDOAQncAICeAPn1AID3ATXUAIDfAOPmAIBiAS3AAIB1AWr8AIDoAMlyAIBKAT7BAIEkATrUAIEvATVYAIE2AQDpAICiAJoAAICPAJIKAIChAcsKAID+AHRwAIC3AFb6AAAA";
		
	    byte[] p1=Base64.decodeBase64(probeTemplate);
		byte[][] c=new byte[4][];
		c[0]=Base64.decodeBase64(t1);
		c[1]=Base64.decodeBase64(t2);
		c[2]=Base64.decodeBase64(t3);
		c[3]=Base64.decodeBase64(t4);
	    /*
	     * Create AFIS Engine and set the Threshold
	     */
		AfisEngine afis = new AfisEngine();
		afis.setThreshold(12);
		
        /*
         * Creating database. More persons can be added to database.
         * Only one person is added to database in this example    
         */
		
		ArrayList<Person> database=new ArrayList<Person>();
		database.add(getPerson(1,c));
		 
		/*giving dummy id -1 for probe*/
		Person probe = getPerson(-1,new byte[][]{p1});
		List<Person> matches=afis.identify(probe, database);
		
		for(Person match:matches){
			System.out.println("Matched::"+match.getId());
		}

	}
	/*
	 * Utility function to create a person from finger print template.
	 */
	static Person getPerson(int id,byte[][] template) throws IOException {
		Fingerprint arrFp[] = new Fingerprint[template.length];
		for(int x=0;x<template.length;x++){	
			arrFp[x] = new Fingerprint();
			arrFp[x].setIsoTemplate(template[x]);
		}
		Person p=new Person(arrFp);
		p.setId(id);
		return p;
	}
}
