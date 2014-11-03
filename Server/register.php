<?php
//电脑端注册页面
//首先检测手机号是否已存在
//其次检测是否在ConfirmMsg表中为未验证
//如果检测无误，将ConfirmMsg表中验证字段设为1
//并且将用户信息添加至UserInfo表

//先检测POST信息完整性
if(!isset($_POST['username'])||!isset($_POST['password'])||!isset($_POST['confirm']))
{
    $error_msg = array('result' => '-1','message' => '注册信息不完全，注册失败！');
	    echo json_encode($error_msg); 
		    exit();
			}
			//赋值
			$username = $_POST['username'];
			$password = $_POST['password'];
			$confirm = $_POST['confirm'];
			//连接数据库
			$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
			if(!$link) 
			{
			  $error_msg = array();
			    $error_msg['result'] = '-1';
				  $error_msg['message'] = "连接失败！原因：" . mysql_error();
				    echo json_encode($error_msg);
					}
					//设置编码
					mysql_query("set names utf8",$link);
					//选择数据库
					if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
					{
					  $error_msg = array();
					    $error_msg['result'] = '-1';
						  $error_msg['message'] = "选择数据库失败！原因：" . mysql_error($link);
						    echo json_encode($error_msg);
							}
							//首先判断用户名是否重复
							$res = mysql_query("select * from UserInfo where username='".$username."'",$link);
							//如果行非空则重复
							if($row = mysql_fetch_assoc($res))
							{
							  $error_msg = array();
							    $error_msg['result'] = '1';
								  $error_msg['message'] = '用户已存在！';
								    echo json_encode($error_msg);
									  exit();
									  }
									  //继续检测ConfirmMsg表
									  $res = mysql_query("select is_confirmed from ConfirmMsg where username = '$username'",$link);
									  $row = mysql_fetch_assoc($res);
									  if($row['is_confirmed'] == 1)
									  {
									    $error_msg = array();
										  $error_msg['result'] = '1';
										    $error_msg['message'] = '用户已存在！';
											  echo json_encode($error_msg);
											    exit();
												}
												$query = mysql_query("select confirm_code from ConfirmMsg where username = '$username'",$link);
												if(!$query)
												{
												  $error_msg = array();
												    $error_msg['result'] = '-1';
													  $error_msg['message'] = mysql_error();
													    echo json_encode($error_msg);
														  exit();
														  }
														  $o = mysql_fetch_assoc($query);
														  if($o['confirm_code'] != $confirm)
														  {
														    $error_msg = array();
															  $error_msg['result'] = '-1';
															    $error_msg['message'] = '验证码错误！';
																  echo json_encode($error_msg);
																    exit();
																	}
																	else
																	{
																	  $res = mysql_query("update ConfirmMsg set is_confirmed = 1 where username='$username'",$link);
																	    if(!$res)
																		  {
																		      $error_msg = array();
																			      $error_msg['result'] = '-1';
																				      $error_msg['message'] = mysql_error();
																					      echo json_encode($error_msg);
																						      exit();
																							    }
																								  //不存在则执行插入语句
																								    $sql = "insert into UserInfo (username,password,is_login) values ('".$username."','".$password."',"."default)";
																									  $res = mysql_query($sql,$link);
																									    if($res)
																										  {
																										      $error_msg = array();
																											      $error_msg['result'] = '0';
																												      $error_msg['message'] = '注册成功！';
																													      echo json_encode($error_msg);
																														    } 
																															  else
																															    {
																																    $error_msg = array();
																																	    $error_msg['result'] = '-1';
																																		    $error_msg['message'] = '注册失败！数据库执行失败！';
																																			    echo json_encode($error_msg);
																																				  }
																																				  }
																																				  ?>
