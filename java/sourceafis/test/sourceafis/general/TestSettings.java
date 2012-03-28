package sourceafis.general;

import java.io.File;

public class TestSettings {
	public static final File folderJavaProject = new File(System.getProperty("user.dir"));
	public static final File folderRoot = new File(new File(folderJavaProject, ".."), "..");
	public static final File folderJavaTestData = new File(new File(folderRoot, "Data"), "JavaTestData");
	
	public static final File fileTemplates = new File(folderJavaTestData, "templates.xml");
	public static final File fileIsoTemplates = new File(folderJavaTestData, "iso.xml");
	public static final File fileScore = new File(folderJavaTestData, "score.xml");
	public static final File fileMatcherLog = new File(folderJavaTestData, "matcher.xml");
	public static final File fileParameters = new File(folderJavaTestData, "parameters.xml");
}
