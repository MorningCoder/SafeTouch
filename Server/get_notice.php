<?php
//获取系统通知
if(!isset($_POST['username']))
{
	$error_msg = array('result' => '-1', 'message' => '信息不完全！');	
		echo json_encode($error_msg);
			exit();
			}

			$username = $_POST['username'];
			//打开数据库连接
			$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);
			if(!$link) 
			{
			    $error_msg = array();
				    $error_msg['result']='-1';
					    $error_msg['message']="连接失败！原因： " . mysql_error();
						    echo json_encode($error_msg);
							    exit();
								}
								//设置字符编码
								mysql_query("set names utf8",$link);
								//选择数据库
								if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
								{
								    $error_msg = array();
									    $error_msg['result']='-1';
										    $error_msg['message']="选择数据库失败！原因：" . mysql_error($link);
											    echo json_encode($error_msg);
												    exit();
													}

													$delete = "delete from Notice where username = '$username'";
													$sql = "select * from Notice where username = '$username'";
													$arr = array();
													$index = 0;

													if($res = mysql_query($sql,$link))
													{
													    if(mysql_num_rows($res) == 0)
															{
															    	$error_msg = array('result' => '*','message' => '');
																	    	echo json_encode($error_msg);
																			    	exit();
																						}
																							while ($row = mysql_fetch_assoc($res)) 
																								{
																										$i = $index++;
																												$arr[$i]['sender'] = $row['sender'];
																														$arr[$i]['content'] = $row['content'];
																															}
																															    //删除记录
																																    if(!mysql_query($delete,$link))
																																	    {
																																		        $error_msg = array('result' => '-1','message' => '删除记录失败！');
																																				    	echo json_encode($error_msg);
																																						        exit();
																																								    }
																																									    //最后返回通知json字符串
																																										    echo json_encode($arr);
																																											} 
																																											else
																																											{
																																												$error_msg = array('result' => '-1','message' => mysql_error());
																																													echo json_encode($error_msg);
																																													}
																																													?>
