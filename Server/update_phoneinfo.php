<?php
//处理上传的手机基本信息
if(!isset($_POST['username'])||!isset($_POST['longtitude'])||
	!isset($_POST['power'])||!isset($_POST['phone_name']) || 
	  	!isset($_POST['latitude']))	
		{
			$error_msg = array();
				$error_msg['result'] = '-1';
					$error_msg['message'] = '信息不完整，上传失败！';
						echo json_encode($error_msg);
							exit();
							}

							$username = $_POST['username'];
							$latitude = $_POST['latitude'];
							$longtitude = $_POST['longtitude'];
							$power = $_POST['power'];
							$phone_name = $_POST['phone_name'];

							$link = @mysql_connect(SAE_MYSQL_HOST_M.':'.SAE_MYSQL_PORT,SAE_MYSQL_USER,SAE_MYSQL_PASS);

							if(!$link) 
							{
								$error_msg = array();
								    $error_msg['result'] = '-1';
									    $error_msg['message'] = "连接失败！原因： " . mysql_error();
										    echo json_encode($error_msg);
											}

											mysql_query("set names utf8",$link);

											if(!mysql_select_db(SAE_MYSQL_DB,$link)) 
											{
											    $error_msg = array();
												    $error_msg['result'] = '-1';
													    $error_msg['message'] = "数据库选择失败！原因： " . mysql_error();
														    echo json_encode($error_msg);
															}
															//先检测是否存在
															$check = "select * from PhoneInfo where username = '$username'";
															$res_check = mysql_query($check,$link);
															//判断是否为空结果集
															if(mysql_num_rows($res_check) == 0)
															{
																$insert = "insert into PhoneInfo values ('$username',$power,'$phone_name','$latitude','$longtitude')";
																	if($r = mysql_query($insert,$link))
																		{
																				$error_msg = array('result' => '0','message' => '上传成功');
																						echo json_encode($error_msg);
																						        exit();
																									}
																										else
																											{
																													$error_msg = array('result' => '-1','message' => '上传失败！原因：'.mysql_error());
																													        echo json_encode($error_msg);
																															        exit();
																																		}
																																		}
																																		else if($res_check)
																																		{
																																		    $update = "update PhoneInfo set power= $power,phone_name = '$phone_name',latitude = '$latitude',longtitude = '$longtitude' where username='$username'";
																																			    if($s = mysql_query($update,$link))
																																					{
																																							$error_msg = array('result' => '0','message' => '上传成功');
																																							        echo json_encode($error_msg);
																																									        exit();
																																												}
																																													else
																																														{
																																																$error_msg = array('result' => '-1','message' => '上传失败！原因：'.mysql_error());
																																																        echo json_encode($error_msg);
																																																		        exit();
																																																					}
																																																					}
																																																					else
																																																					{
																																																						$error_msg = array('result' => '-1','message' => '上传失败！原因：'.mysql_error());
																																																						    echo json_encode($error_msg);
																																																							    exit();
																																																								}
																																																								?>
