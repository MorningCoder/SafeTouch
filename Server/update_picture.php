<?php
//处理手机端上传的摄像头截图
if(!isset($_POST['username'])||!isset($_POST['time'])) 
{
  $error_msg = array();
    $error_msg['result'] = '-1';
	  $error_msg['message'] = '信息不完整，上传失败！';
	    echo json_encode($error_msg);
		  exit();
		  }

		  $picture_name = $_FILES['picture']['name'];
		  $username = $_POST['username'];
		  $time = $_POST['time'];

		  if(is_uploaded_file($_FILES['picture']['tmp_name']))
		  {
		    $uploaded_file = $_FILES['picture']['tmp_name'];
			  $fp = fopen($uploaded_file,"r");
			    $data = addslashes(fread($fp,filesize($uploaded_file)));
				  
				    $link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
					  if(!$link) 
					    {
						    $error_msg = array('result' => '-1','message' => '数据库连接失败'.mysql_error());
							    echo json_encode($error_msg);
								  }
								    
									  mysql_query("set names utf8",$link);
									    if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
										  {
										      $error_msg = array('result' => '-1','message' => '选择数据库失败');
											      echo json_encode($error_msg);
												    } 
													  
													    $sql = "insert into Photo values ('$username','$data','$time','$picture_name')";
														  
														    $res = mysql_query($sql,$link);
															  if($res)
															    {
																    $error_msg = array('result' => '0', 'message' => '上传成功');
																	    echo json_encode($error_msg);
																		  }
																		    else
																			  {
																			      $error_msg = array('result' => '-1', 'message' => '上传失败！数据库执行失败！原因：'.mysql_error());
																				      echo json_encode($error_msg);
																					    }
																						}
																						else
																						{
																						    $error_msg = array('result' => '-1', 'message' => '上传失败！文件未接受');
																							    echo json_encode($error_msg);
																								}
																								?>
